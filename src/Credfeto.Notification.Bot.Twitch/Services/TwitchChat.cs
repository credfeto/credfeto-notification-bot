using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.StreamState;
using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NonBlocking;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchChat : ITwitchChat, IDisposable
{
    private const string ANONYMOUS_CHEERER_USERNAME = "ananonymouscheerer";
    private readonly IDisposable _channelJoined;
    private readonly IDisposable _channelRaided;
    private readonly IDisposable _channelStateChanged;
    private readonly IDisposable _chatCleared;
    private readonly IDisposable _chatConnected;
    private readonly IDisposable _chatDisconnected;
    private readonly IDisposable? _chatLogMessage;
    private readonly IDisposable _chatMessageReceived;
    private readonly IDisposable _chatReconnected;
    private readonly TwitchClient _client;
    private readonly IDisposable _communityGiftSub;
    private readonly IDisposable _continuedGiftedSub;
    private readonly IDisposable _giftedSub;

    private readonly ConcurrentDictionary<Streamer, bool> _joinedStreamers;
    private readonly ConcurrentDictionary<Streamer, string> _lastMessage;
    private readonly SemaphoreSlim _lastMessageLock;
    private readonly ILogger<TwitchChat> _logger;
    private readonly IMediator _mediator;
    private readonly IDisposable _newSubscriber;

    private readonly TwitchBotOptions _options;
    private readonly IDisposable _primeToPaidSub;
    private readonly IDisposable _reSubscription;
    private readonly IDisposable _sentChatMessages;
    private readonly ITwitchChannelManager _twitchChannelManager;

    [SuppressMessage(category: "ReSharper", checkId: "PrivateFieldCanBeConvertedToLocalVariable", Justification = "TODO: Review")]
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    private bool _connected;

    public TwitchChat(IOptions<TwitchBotOptions> options,
                      ITwitchChannelManager twitchChannelManager,
                      IMessageChannel<TwitchChatMessage> twitchChatMessageChannel,
                      IMediator mediator,
                      ITwitchClient twitchClient,
                      ILogger<TwitchChat> logger)
    {
        this._twitchChannelManager = twitchChannelManager ?? throw new ArgumentNullException(nameof(twitchChannelManager));
        this._twitchChatMessageChannel = twitchChatMessageChannel;
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        this._client = twitchClient as TwitchClient ?? throw new ArgumentNullException(nameof(twitchClient));

        this._lastMessageLock = new(initialCount: 1, maxCount: 1);
        this._joinedStreamers = new();
        this._lastMessage = new();

        ConnectionCredentials credentials = new(twitchUsername: this._options.Authentication.UserName, twitchOAuth: this._options.Authentication.OAuthToken);

        this._client.Initialize(credentials: credentials, channels: new() { this._options.Authentication.UserName });

        this._joinedStreamers.TryAdd(Streamer.FromString(this._options.Authentication.UserName), value: true);

        // HEALTH
        this._chatConnected = this.SubscribeToChatConnection();
        this._chatDisconnected = this.SubscribeToChatDisconnection();
        this._chatReconnected = this.SubscribeToChatReconnections();
        this._channelJoined = this.SubscribeToChannelJoined();

        // STATE
        this._channelStateChanged = this.SubscribeToChannelStateChanged();

        // LOGGING
        this._chatLogMessage = this._logger.IsEnabled(LogLevel.Debug)
            ? this.SubscribeToChatLogMessages()
            : null;

        // CHAT
        this._chatMessageReceived = this.SubscribeToIncomingChatMessages();
        this._chatCleared = this.SubscribeToChatCleared();

        // RAIDS
        this._channelRaided = this.SubscribeToChannelRaided();

        // SUBS
        this._newSubscriber = this.SubscribeToNewSubscriberMessages();
        this._reSubscription = this.SubscribeToReSubscriptionMessages();
        this._communityGiftSub = this.SubscribeToCommunityGiftSubs();
        this._giftedSub = this.SubscribeToGiftedSub();
        this._continuedGiftedSub = this.SubscribeToContinuedGiftedSub();
        this._primeToPaidSub = this.SubscribeToPrimeToPaidSubConversions();

        // MESSAGES BEING SENT
        this._sentChatMessages = this.SubscribeToOutgoingChatMessages();

        this._client.Connect();
        this._connected = true;

        this._client.JoinChannel(this._options.Authentication.UserName);
    }

    public void Dispose()
    {
        this._channelJoined.Dispose();
        this._channelRaided.Dispose();
        this._channelStateChanged.Dispose();
        this._chatCleared.Dispose();
        this._chatConnected.Dispose();
        this._chatDisconnected.Dispose();
        this._chatLogMessage?.Dispose();
        this._chatMessageReceived.Dispose();
        this._chatReconnected.Dispose();
        this._communityGiftSub.Dispose();
        this._continuedGiftedSub.Dispose();
        this._giftedSub.Dispose();
        this._lastMessageLock.Dispose();
        this._newSubscriber.Dispose();
        this._primeToPaidSub.Dispose();
        this._reSubscription.Dispose();
        this._sentChatMessages.Dispose();
    }

    public void JoinChat(Streamer streamer)
    {
        this._joinedStreamers.TryAdd(key: streamer, value: true);

        this._client.JoinChannel(streamer.Value);
    }

    public void LeaveChat(Streamer streamer)
    {
        if (StringComparer.InvariantCultureIgnoreCase.Equals(x: this._options.Authentication.UserName, y: streamer.Value))
        {
            // never leave own channel.
            return;
        }

        this._joinedStreamers.TryRemove(key: streamer, out bool _);

        this._client.LeaveChannel(streamer.Value);
    }

    public Task UpdateAsync()
    {
        if (!this._connected)
        {
            this._logger.LogDebug("Chat Reconnecting...");

            //this._client.Connect();
            this._connected = true;
        }

        this.ReconnectToJoinedChats();

        return Task.CompletedTask;
    }

    private IDisposable SubscribeToOutgoingChatMessages()
    {
        return this._twitchChatMessageChannel.ReadAllAsync(CancellationToken.None)
                   .ToObservable()
                   .Delay(d => Observable.Timer(this.CalculateWithJitter(d)))
                   .Where(this.IsConnectedToChat)
                   .Select(message => Observable.FromAsync(cancellationToken => this.PublishChatMessageAsync(twitchChatMessage: message, cancellationToken: cancellationToken)))
                   .Concat()
                   .Subscribe();
    }

    private bool IsMessageForModChannel(string streamer, string viewer)
    {
        return !this._options.IsSelf(Viewer.FromString(viewer)) && this._options.IsModChannel(Streamer.FromString(streamer));
    }

    [SuppressMessage(category: "Philips.CodeAnalysis.DuplicateCodeAnalyzer", checkId: "PH2071: Duplicate code segment", Justification = "TODO: Optimise")]
    private IDisposable SubscribeToPrimeToPaidSubConversions()
    {
        return Observable.FromEventPattern<OnPrimePaidSubscriberArgs>(addHandler: h => this._client.OnPrimePaidSubscriber += h, removeHandler: h => this._client.OnPrimePaidSubscriber -= h)
                         .Select(messageEvent => messageEvent.EventArgs)
                         .Where(e => this.IsMessageForModChannel(streamer: e.Channel, viewer: e.PrimePaidSubscriber.DisplayName))
                         .Select(e => Observable.FromAsync(cancellationToken => this.OnPrimePaidSubscriberAsync(e: e, cancellationToken: cancellationToken)))
                         .Concat()
                         .Subscribe();
    }

    [SuppressMessage(category: "Philips.CodeAnalysis.DuplicateCodeAnalyzer", checkId: "PH2071: Duplicate code segment", Justification = "TODO: Optimise")]
    private IDisposable SubscribeToContinuedGiftedSub()
    {
        return Observable.FromEventPattern<OnContinuedGiftedSubscriptionArgs>(addHandler: h => this._client.OnContinuedGiftedSubscription += h,
                                                                              removeHandler: h => this._client.OnContinuedGiftedSubscription -= h)
                         .Select(messageEvent => messageEvent.EventArgs)
                         .Where(e => this.IsMessageForModChannel(streamer: e.Channel, viewer: e.ContinuedGiftedSubscription.DisplayName))
                         .Select(e => Observable.FromAsync(cancellationToken => this.OnContinuedGiftedSubscriptionAsync(e: e, cancellationToken: cancellationToken)))
                         .Concat()
                         .Subscribe();
    }

    [SuppressMessage(category: "Philips.CodeAnalysis.DuplicateCodeAnalyzer", checkId: "PH2071: Duplicate code segment", Justification = "TODO: Optimise")]
    private IDisposable SubscribeToGiftedSub()
    {
        return Observable.FromEventPattern<OnGiftedSubscriptionArgs>(addHandler: h => this._client.OnGiftedSubscription += h, removeHandler: h => this._client.OnGiftedSubscription -= h)
                         .Select(messageEvent => messageEvent.EventArgs)
                         .Where(e => this.IsMessageForModChannel(streamer: e.Channel, viewer: e.GiftedSubscription.DisplayName))
                         .Select(e => Observable.FromAsync(cancellationToken => this.OnGiftedSubscriptionAsync(e: e, cancellationToken: cancellationToken)))
                         .Concat()
                         .Subscribe();
    }

    [SuppressMessage(category: "Philips.CodeAnalysis.DuplicateCodeAnalyzer", checkId: "PH2071: Duplicate code segment", Justification = "TODO: Optimise")]
    private IDisposable SubscribeToCommunityGiftSubs()
    {
        return Observable.FromEventPattern<OnCommunitySubscriptionArgs>(addHandler: h => this._client.OnCommunitySubscription += h, removeHandler: h => this._client.OnCommunitySubscription -= h)
                         .Select(messageEvent => messageEvent.EventArgs)
                         .Where(e => this.IsMessageForModChannel(streamer: e.Channel, viewer: e.GiftedSubscription.DisplayName))
                         .Select(e => Observable.FromAsync(cancellationToken => this.OnCommunitySubscriptionAsync(e: e, cancellationToken: cancellationToken)))
                         .Concat()
                         .Subscribe();
    }

    [SuppressMessage(category: "Philips.CodeAnalysis.DuplicateCodeAnalyzer", checkId: "PH2071: Duplicate code segment", Justification = "TODO: Optimise")]
    private IDisposable SubscribeToReSubscriptionMessages()
    {
        return Observable.FromEventPattern<OnReSubscriberArgs>(addHandler: h => this._client.OnReSubscriber += h, removeHandler: h => this._client.OnReSubscriber -= h)
                         .Select(messageEvent => messageEvent.EventArgs)
                         .Where(e => this.IsMessageForModChannel(streamer: e.Channel, viewer: e.ReSubscriber.DisplayName))
                         .Select(e => Observable.FromAsync(cancellationToken => this.OnReSubscribeAsync(e: e, cancellationToken: cancellationToken)))
                         .Concat()
                         .Subscribe();
    }

    [SuppressMessage(category: "Philips.CodeAnalysis.DuplicateCodeAnalyzer", checkId: "PH2071: Duplicate code segment", Justification = "TODO: Optimise")]
    private IDisposable SubscribeToNewSubscriberMessages()
    {
        return Observable.FromEventPattern<OnNewSubscriberArgs>(addHandler: h => this._client.OnNewSubscriber += h, removeHandler: h => this._client.OnNewSubscriber -= h)
                         .Select(messageEvent => messageEvent.EventArgs)
                         .Where(e => this.IsMessageForModChannel(streamer: e.Channel, viewer: e.Subscriber.DisplayName))
                         .Select(e => Observable.FromAsync(cancellationToken => this.OnNewSubscriberAsync(e: e, cancellationToken: cancellationToken)))
                         .Concat()
                         .Subscribe();
    }

    private IDisposable SubscribeToChannelRaided()
    {
        return Observable.FromEventPattern<OnRaidNotificationArgs>(addHandler: h => this._client.OnRaidNotification += h, removeHandler: h => this._client.OnRaidNotification -= h)
                         .Select(messageEvent => messageEvent.EventArgs)
                         .Where(e => this._options.IsModChannel(Streamer.FromString(e.Channel)))
                         .Select(e => Observable.FromAsync(cancellationToken => this.OnRaidAsync(e: e, cancellationToken: cancellationToken)))
                         .Concat()
                         .Subscribe();
    }

    private IDisposable SubscribeToChatCleared()
    {
        return Observable.FromEventPattern<OnChatClearedArgs>(addHandler: h => this._client.OnChatCleared += h, removeHandler: h => this._client.OnChatCleared -= h)
                         .Select(messageEvent => messageEvent.EventArgs)
                         .Where(e => this._options.IsModChannel(Streamer.FromString(e.Channel)))
                         .Subscribe(onNext: this.Client_OnChatCleared);
    }

    private IDisposable SubscribeToIncomingChatMessages()
    {
        return Observable.FromEventPattern<OnMessageReceivedArgs>(addHandler: h => this._client.OnMessageReceived += h, removeHandler: h => this._client.OnMessageReceived -= h)
                         .Select(messageEvent => messageEvent.EventArgs)
                         .Select(e => Observable.FromAsync(cancellationToken => this.OnMessageReceivedAsync(e: e, cancellationToken: cancellationToken)))
                         .Concat()
                         .Subscribe();
    }

    private IDisposable SubscribeToChatLogMessages()
    {
        return Observable.FromEventPattern<OnLogArgs>(addHandler: h => this._client.OnLog += h, removeHandler: h => this._client.OnLog -= h)
                         .Select(messageEvent => messageEvent.EventArgs)
                         .Subscribe(onNext: this.OnLog);
    }

    private IDisposable SubscribeToChannelStateChanged()
    {
        return Observable.FromEventPattern<OnChannelStateChangedArgs>(addHandler: h => this._client.OnChannelStateChanged += h, removeHandler: h => this._client.OnChannelStateChanged -= h)
                         .Select(messageEvent => messageEvent.EventArgs)
                         .Subscribe(onNext: this.Client_OnChannelStateChanged);
    }

    private IDisposable SubscribeToChannelJoined()
    {
        return Observable.FromEventPattern<OnJoinedChannelArgs>(addHandler: h => this._client.OnJoinedChannel += h, removeHandler: h => this._client.OnJoinedChannel -= h)
                         .Select(messageEvent => messageEvent.EventArgs)
                         .Select(e => Observable.FromAsync(cancellationToken => this.OnJoinedChannelAsync(e: e, cancellationToken: cancellationToken)))
                         .Concat()
                         .Subscribe();
    }

    private IDisposable SubscribeToChatReconnections()
    {
        return Observable.FromEventPattern<OnReconnectedEventArgs>(addHandler: h => this._client.OnReconnected += h, removeHandler: h => this._client.OnReconnected -= h)
                         .Select(messageEvent => messageEvent.EventArgs)
                         .Subscribe(onNext: this.OnReconnected);
    }

    private IDisposable SubscribeToChatDisconnection()
    {
        return Observable.FromEventPattern<OnDisconnectedEventArgs>(addHandler: h => this._client.OnDisconnected += h, removeHandler: h => this._client.OnDisconnected -= h)
                         .Select(messageEvent => messageEvent.EventArgs)
                         .Subscribe(onNext: this.OnDisconnected);
    }

    private IDisposable SubscribeToChatConnection()
    {
        return Observable.FromEventPattern<OnConnectedArgs>(addHandler: h => this._client.OnConnected += h, removeHandler: h => this._client.OnConnected -= h)
                         .Select(messageEvent => messageEvent.EventArgs)
                         .Subscribe(onNext: this.OnConnected);
    }

    private TimeSpan CalculateWithJitter(TwitchChatMessage twitchChatMessage)
    {
        static int GetMaxSeconds(MessagePriority messagePriority)
        {
            return messagePriority switch
            {
                MessagePriority.ASAP => 2 * (int)MessagePriority.ASAP,
                MessagePriority.NATURAL => 3 * (int)MessagePriority.NATURAL,
                MessagePriority.SLOW => 6 * (int)MessagePriority.SLOW,
                _ => throw new ArgumentOutOfRangeException(nameof(messagePriority), actualValue: messagePriority, message: "Unknown message priority")
            };
        }

        MessagePriority priority = twitchChatMessage.Priority;

        double delay = Jitter.WithJitter((int)priority, GetMaxSeconds(priority));

        this._logger.LogInformation($"{twitchChatMessage.Streamer}: Delaying message for {delay} seconds for: {twitchChatMessage.Message}");

        return TimeSpan.FromSeconds(delay);
    }

    private bool IsConnectedToChat(TwitchChatMessage chatMessage)
    {
        return this.IsConnectedToChat(chatMessage.Streamer);
    }

    private bool IsConnectedToChat(Streamer streamer)
    {
        return this._client.JoinedChannels.Any(joinedChannel => StringComparer.InvariantCultureIgnoreCase.Equals(x: streamer.Value, y: joinedChannel.Channel));
    }

    private async Task PublishChatMessageAsync(TwitchChatMessage twitchChatMessage, CancellationToken cancellationToken)
    {
        await this._lastMessageLock.WaitAsync(cancellationToken);

        try
        {
            if (this._lastMessage.TryGetValue(key: twitchChatMessage.Streamer, out string? lastMessage) &&
                StringComparer.InvariantCultureIgnoreCase.Equals(x: lastMessage, y: twitchChatMessage.Message))
            {
                if (!twitchChatMessage.Message.StartsWith(value: "!", comparisonType: StringComparison.Ordinal) &&
                    !twitchChatMessage.Message.StartsWith(value: "/", comparisonType: StringComparison.Ordinal))
                {
                    return;
                }
            }

            this._lastMessage.TryRemove(key: twitchChatMessage.Streamer, value: out _);

            this._logger.LogInformation($"{twitchChatMessage.Streamer}: >>> {this._options.Authentication.UserName} SEND >>> {twitchChatMessage.Message}");

            this._client.SendMessage(channel: twitchChatMessage.Streamer.Value, message: twitchChatMessage.Message);

            this._lastMessage.TryAdd(key: twitchChatMessage.Streamer, value: twitchChatMessage.Message);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{twitchChatMessage.Streamer}: Failed to publish message : {exception.Message}");
        }
        finally
        {
            this._lastMessageLock.Release();
        }
    }

    private void Client_OnChatCleared(OnChatClearedArgs e)
    {
        Streamer streamer = Streamer.FromString(e.Channel);

        if (!this._options.IsModChannel(streamer))
        {
            return;
        }

        this._logger.LogInformation($"{streamer}: Chat Cleared");

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(streamer);

        state.ClearChat();
    }

    private async Task OnCommunitySubscriptionAsync(OnCommunitySubscriptionArgs e, CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.Channel);
        this._logger.LogInformation($"{streamer}: Community Sub: {e.GiftedSubscription.DisplayName}");

        if (e.GiftedSubscription.IsAnonymous)
        {
            return;
        }

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(streamer);

        await state.GiftedMultipleAsync(Viewer.FromString(e.GiftedSubscription.DisplayName),
                                        count: e.GiftedSubscription.MsgParamMassGiftCount,
                                        months: e.GiftedSubscription.MsgParamMultiMonthGiftDuration,
                                        cancellationToken: cancellationToken);
    }

    private async Task OnGiftedSubscriptionAsync(OnGiftedSubscriptionArgs e, CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.Channel);
        this._logger.LogInformation($"{streamer}: Community Sub: {e.GiftedSubscription.DisplayName}");

        if (e.GiftedSubscription.IsAnonymous)
        {
            return;
        }

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(streamer);

        await state.GiftedSubAsync(Viewer.FromString(e.GiftedSubscription.DisplayName), months: e.GiftedSubscription.MsgParamMultiMonthGiftDuration, cancellationToken: cancellationToken);
    }

    private void Client_OnChannelStateChanged(OnChannelStateChangedArgs e)
    {
        Streamer streamer = Streamer.FromString(e.Channel);

        if (!this._options.IsModChannel(streamer))
        {
            return;
        }

        this._logger.LogInformation($"{streamer}: Emote Only: {e.ChannelState.EmoteOnly} Follower Only: {e.ChannelState.FollowersOnly} Sub Only: {e.ChannelState.SubOnly}");
    }

    private async Task OnContinuedGiftedSubscriptionAsync(OnContinuedGiftedSubscriptionArgs e, CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.Channel);
        this._logger.LogInformation($"{e.Channel}: {e.ContinuedGiftedSubscription.DisplayName} continued sub gifted by {e.ContinuedGiftedSubscription.MsgParamSenderLogin}");

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(streamer);

        await state.ContinuedSubAsync(Viewer.FromString(e.ContinuedGiftedSubscription.DisplayName), cancellationToken: cancellationToken);
    }

    private async Task OnPrimePaidSubscriberAsync(OnPrimePaidSubscriberArgs e, CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.Channel);
        this._logger.LogInformation($"{streamer}: {e.PrimePaidSubscriber.DisplayName} converted prime sub to paid");

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(streamer);

        await state.PrimeToPaidAsync(Viewer.FromString(e.PrimePaidSubscriber.DisplayName), cancellationToken: cancellationToken);
    }

    private void OnLog(OnLogArgs e)
    {
        this._logger.LogDebug($"{e.DateTime}: {e.BotUsername} - {e.Data}");
    }

    private void OnConnected(OnConnectedArgs e)
    {
        this._logger.LogWarning($"Connected to {e.BotUsername} {e.AutoJoinChannel}");
        this._connected = true;
    }

    private void OnDisconnected(OnDisconnectedEventArgs e)
    {
        this._logger.LogWarning("Chat Disconnected :(");
        this._connected = false;
    }

    private void OnReconnected(OnReconnectedEventArgs e)
    {
        this._logger.LogWarning("Chat Reconnected :)");
        this._connected = true;

        this.ReconnectToJoinedChats();
    }

    private void ReconnectToJoinedChats()
    {
        // Join all the previously joined channels if not already connected
        IReadOnlyList<Streamer> streamers = this._joinedStreamers.Keys.Where(streamer => !this.IsConnectedToChat(streamer))
                                                .ToArray();

        foreach (Streamer streamer in streamers)
        {
            this._logger.LogInformation($"{streamer}: Reconnecting to chat...");
            this.JoinChat(streamer);
        }
    }

    private async Task OnRaidAsync(OnRaidNotificationArgs e, CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.Channel);
        this._logger.LogInformation($"{streamer}: Raided by {e.RaidNotification.DisplayName}");

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(streamer);

        if (!int.TryParse(s: e.RaidNotification.MsgParamViewerCount, style: NumberStyles.Integer, provider: CultureInfo.InvariantCulture, out int viewerCount))
        {
            viewerCount = 1;
        }

        await state.RaidedAsync(Viewer.FromString(e.RaidNotification.DisplayName), viewerCount: viewerCount, cancellationToken: cancellationToken);
    }

    private async Task OnJoinedChannelAsync(OnJoinedChannelArgs e, CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.Channel);
        this._logger.LogInformation($"{streamer}: Joining channel as {e.BotUsername}");

        if (!this._options.IsModChannel(streamer))
        {
            return;
        }

        try
        {
            await this._mediator.Publish(new TwitchChannelChatConnected(streamer), cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError($"{e.Channel}: Failed to initialise: {exception.Message}");
        }
    }

    private async Task OnMessageReceivedAsync(OnMessageReceivedArgs e, CancellationToken cancellationToken)
    {
        if (StringComparer.InvariantCultureIgnoreCase.Equals(x: e.ChatMessage.Username, y: ANONYMOUS_CHEERER_USERNAME))
        {
            // Skip anonymous cheerers
            return;
        }

        Streamer streamer = Streamer.FromString(e.ChatMessage.Channel);
        Viewer viewer = Viewer.FromString(e.ChatMessage.Username);

        string message = e.ChatMessage.Message;
        await this.HandleChatMessageAsync(streamer: streamer, viewer: viewer, message: message, cancellationToken: cancellationToken);

        if (this._options.IsSelf(viewer))
        {
            // skip messages from self
            return;
        }

        int bits = e.ChatMessage.Bits;

        if (bits > 0)
        {
            await this.HandleBitsGiftAsync(streamer: streamer, viewer: viewer, bits: bits, cancellationToken: cancellationToken);
        }
    }

    private async Task HandleBitsGiftAsync(Streamer streamer, Viewer viewer, int bits, CancellationToken cancellationToken)
    {
        if (!this._options.IsModChannel(streamer))
        {
            return;
        }

        try
        {
            ITwitchChannelState channelState = this._twitchChannelManager.GetStreamer(streamer);

            await channelState.BitsGiftedAsync(user: viewer, bits: bits, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError($"{streamer}: Failed to handle bit gift: {exception.Message}");
        }
    }

    private async Task HandleChatMessageAsync(Streamer streamer, Viewer viewer, string message, CancellationToken cancellationToken)
    {
        try
        {
            TwitchIncomingMessage incomingMessage = new(Streamer: streamer, Chatter: viewer, Message: message);
            await this._mediator.Publish(notification: incomingMessage, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError($"{streamer}: Failed to handle chat message: {exception.Message}");
        }
    }

    private async Task OnNewSubscriberAsync(OnNewSubscriberArgs e, CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.Channel);

        this._logger.LogInformation($"{streamer}: New Subscriber {e.Subscriber.DisplayName}");

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(streamer);

        if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
        {
            await state.NewSubscriberPaidAsync(Viewer.FromString(e.Subscriber.DisplayName), cancellationToken: cancellationToken);

            return;
        }

        await state.NewSubscriberPrimeAsync(Viewer.FromString(e.Subscriber.DisplayName), cancellationToken: cancellationToken);
    }

    private async Task OnReSubscribeAsync(OnReSubscriberArgs e, CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.Channel);
        this._logger.LogInformation($"{streamer}: Resub {e.ReSubscriber.DisplayName} for {e.ReSubscriber.Months}");

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(streamer);

        if (e.ReSubscriber.SubscriptionPlan == SubscriptionPlan.Prime)
        {
            await state.ResubscribePaidAsync(Viewer.FromString(e.ReSubscriber.DisplayName), months: e.ReSubscriber.Months, cancellationToken: cancellationToken);

            return;
        }

        await state.ResubscribePrimeAsync(Viewer.FromString(e.ReSubscriber.DisplayName), months: e.ReSubscriber.Months, cancellationToken: cancellationToken);
    }
}