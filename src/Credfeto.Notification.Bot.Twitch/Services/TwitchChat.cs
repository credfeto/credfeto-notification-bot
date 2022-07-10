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
using MediatR;
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

public sealed class TwitchChat : ITwitchChat
{
    private readonly TwitchClient _client;

    private readonly ConcurrentDictionary<Streamer, bool> _joinedStreamers;
    private readonly ConcurrentDictionary<Streamer, string> _lastMessage;
    private readonly SemaphoreSlim _lastMessageLock;
    private readonly ILogger<TwitchChat> _logger;
    private readonly IMediator _mediator;

    private readonly TwitchBotOptions _options;
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
        Observable.FromEventPattern<OnConnectedArgs>(addHandler: h => this._client.OnConnected += h, removeHandler: h => this._client.OnConnected -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(onNext: this.OnConnected);

        Observable.FromEventPattern<OnDisconnectedEventArgs>(addHandler: h => this._client.OnDisconnected += h, removeHandler: h => this._client.OnDisconnected -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(onNext: this.OnDisconnected);

        Observable.FromEventPattern<OnReconnectedEventArgs>(addHandler: h => this._client.OnReconnected += h, removeHandler: h => this._client.OnReconnected -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(onNext: this.OnReconnected);

        Observable.FromEventPattern<OnJoinedChannelArgs>(addHandler: h => this._client.OnJoinedChannel += h, removeHandler: h => this._client.OnJoinedChannel -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnJoinedChannelAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        // STATE
        Observable.FromEventPattern<OnChannelStateChangedArgs>(addHandler: h => this._client.OnChannelStateChanged += h, removeHandler: h => this._client.OnChannelStateChanged -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(onNext: this.Client_OnChannelStateChanged);

        // LOGGING
        if (this._logger.IsEnabled(LogLevel.Debug))
        {
            Observable.FromEventPattern<OnLogArgs>(addHandler: h => this._client.OnLog += h, removeHandler: h => this._client.OnLog -= h)
                      .Select(messageEvent => messageEvent.EventArgs)
                      .Subscribe(onNext: this.OnLog);
        }

        // CHAT
        Observable.FromEventPattern<OnMessageReceivedArgs>(addHandler: h => this._client.OnMessageReceived += h, removeHandler: h => this._client.OnMessageReceived -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnMessageReceivedAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnChatClearedArgs>(addHandler: h => this._client.OnChatCleared += h, removeHandler: h => this._client.OnChatCleared -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => this._options.IsModChannel(Streamer.FromString(e.Channel)))
                  .Subscribe(onNext: this.Client_OnChatCleared);

        // RAIDS
        Observable.FromEventPattern<OnRaidNotificationArgs>(addHandler: h => this._client.OnRaidNotification += h, removeHandler: h => this._client.OnRaidNotification -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => this._options.IsModChannel(Streamer.FromString(e.Channel)))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnRaidAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        // SUBS
        Observable.FromEventPattern<OnNewSubscriberArgs>(addHandler: h => this._client.OnNewSubscriber += h, removeHandler: h => this._client.OnNewSubscriber -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => !this._options.IsSelf(Viewer.FromString(e.Subscriber.DisplayName)))
                  .Where(e => this._options.IsModChannel(Streamer.FromString(e.Channel)))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnNewSubscriberAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnReSubscriberArgs>(addHandler: h => this._client.OnReSubscriber += h, removeHandler: h => this._client.OnReSubscriber -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => !this._options.IsSelf(Viewer.FromString(e.ReSubscriber.DisplayName)))
                  .Where(e => this._options.IsModChannel(Streamer.FromString(e.Channel)))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnReSubscribeAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnCommunitySubscriptionArgs>(addHandler: h => this._client.OnCommunitySubscription += h, removeHandler: h => this._client.OnCommunitySubscription -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => !this._options.IsSelf(Viewer.FromString(e.GiftedSubscription.DisplayName)))
                  .Where(e => this._options.IsModChannel(Streamer.FromString(e.Channel)))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnCommunitySubscriptionAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnGiftedSubscriptionArgs>(addHandler: h => this._client.OnGiftedSubscription += h, removeHandler: h => this._client.OnGiftedSubscription -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => !this._options.IsSelf(Viewer.FromString(e.GiftedSubscription.DisplayName)))
                  .Where(e => this._options.IsModChannel(Streamer.FromString(e.Channel)))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnGiftedSubscriptionAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnContinuedGiftedSubscriptionArgs>(addHandler: h => this._client.OnContinuedGiftedSubscription += h,
                                                                       removeHandler: h => this._client.OnContinuedGiftedSubscription -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => !this._options.IsSelf(Viewer.FromString(e.ContinuedGiftedSubscription.DisplayName)))
                  .Where(e => this._options.IsModChannel(Streamer.FromString(e.Channel)))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnContinuedGiftedSubscriptionAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnPrimePaidSubscriberArgs>(addHandler: h => this._client.OnPrimePaidSubscriber += h, removeHandler: h => this._client.OnPrimePaidSubscriber -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => !this._options.IsSelf(Viewer.FromString(e.PrimePaidSubscriber.DisplayName)))
                  .Where(e => this._options.IsModChannel(Streamer.FromString(e.Channel)))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnPrimePaidSubscriberAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        this._twitchChatMessageChannel.ReadAllAsync(CancellationToken.None)
            .ToObservable()
            .Delay(d => Observable.Timer(this.CalculateWithJitter(d)))
            .Where(this.IsConnectedToChat)
            .Select(message => Observable.FromAsync(cancellationToken => this.PublishChatMessageAsync(twitchChatMessage: message, cancellationToken: cancellationToken)))
            .Concat()
            .Subscribe();

        this._client.Connect();
        this._connected = true;

        this._client.JoinChannel(this._options.Authentication.UserName);
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

        this._logger.LogDebug($"{twitchChatMessage.Streamer}: Delaying message for {delay} seconds for: {twitchChatMessage.Message}");

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
                return;
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

    private Task OnCommunitySubscriptionAsync(OnCommunitySubscriptionArgs e, in CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.Channel);
        this._logger.LogInformation($"{streamer}: Community Sub: {e.GiftedSubscription.DisplayName}");

        if (e.GiftedSubscription.IsAnonymous)
        {
            return Task.CompletedTask;
        }

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(streamer);

        return state.GiftedMultipleAsync(Viewer.FromString(e.GiftedSubscription.DisplayName),
                                         count: e.GiftedSubscription.MsgParamMassGiftCount,
                                         months: e.GiftedSubscription.MsgParamMultiMonthGiftDuration,
                                         cancellationToken: cancellationToken);
    }

    private Task OnGiftedSubscriptionAsync(OnGiftedSubscriptionArgs e, in CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.Channel);
        this._logger.LogInformation($"{streamer}: Community Sub: {e.GiftedSubscription.DisplayName}");

        if (e.GiftedSubscription.IsAnonymous)
        {
            return Task.CompletedTask;
        }

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(streamer);

        return state.GiftedSubAsync(Viewer.FromString(e.GiftedSubscription.DisplayName), months: e.GiftedSubscription.MsgParamMultiMonthGiftDuration, cancellationToken: cancellationToken);
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

    private Task OnContinuedGiftedSubscriptionAsync(OnContinuedGiftedSubscriptionArgs e, in CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.Channel);
        this._logger.LogInformation($"{e.Channel}: {e.ContinuedGiftedSubscription.DisplayName} continued sub gifted by {e.ContinuedGiftedSubscription.MsgParamSenderLogin}");

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(streamer);

        return state.ContinuedSubAsync(Viewer.FromString(e.ContinuedGiftedSubscription.DisplayName), cancellationToken: cancellationToken);
    }

    private Task OnPrimePaidSubscriberAsync(OnPrimePaidSubscriberArgs e, in CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.Channel);
        this._logger.LogInformation($"{streamer}: {e.PrimePaidSubscriber.DisplayName} converted prime sub to paid");

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(streamer);

        return state.PrimeToPaidAsync(Viewer.FromString(e.PrimePaidSubscriber.DisplayName), cancellationToken: cancellationToken);
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

    private Task OnRaidAsync(OnRaidNotificationArgs e, in CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.Channel);
        this._logger.LogInformation($"{streamer}: Raided by {e.RaidNotification.DisplayName}");

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(streamer);

        if (!int.TryParse(s: e.RaidNotification.MsgParamViewerCount, style: NumberStyles.Integer, provider: CultureInfo.InvariantCulture, out int viewerCount))
        {
            viewerCount = 1;
        }

        return state.RaidedAsync(Viewer.FromString(e.RaidNotification.DisplayName), viewerCount: viewerCount, cancellationToken: cancellationToken);
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
        if (StringComparer.InvariantCultureIgnoreCase.Equals(x: e.ChatMessage.Username, y: this._options.Authentication.UserName))
        {
            return;
        }

        if (this._options.Heists.Contains(e.ChatMessage.Channel))
        {
            if (await this.JoinHeistAsync(e: e, cancellationToken: cancellationToken))
            {
                // It was a heist message, no point in processing anything else.
                return;
            }
        }

        Streamer streamer = Streamer.FromString(e.ChatMessage.Channel);

        TwitchMarbles? marbles = this._options.Marbles?.FirstOrDefault(x => IsHeistMessage(message: e, marbles: x));

        if (marbles != null)
        {
            // It was a heist message, no point in processing anything else.
            this._logger.LogWarning($"{e.ChatMessage.Channel}: Marbles detected from user: {e.ChatMessage.Username}");

            await this.JoinMarblesGameAsync(streamer: streamer, cancellationToken: cancellationToken);

            return;
        }

        if (!this._options.IsModChannel(streamer))
        {
            return;
        }

        this._logger.LogInformation($"{streamer}: @{e.ChatMessage.Username}: {e.ChatMessage.Message}");

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(streamer);

        await state.ChatMessageAsync(Viewer.FromString(e.ChatMessage.Username), message: e.ChatMessage.Message, bits: e.ChatMessage.Bits, cancellationToken: cancellationToken);
    }

    private Task JoinMarblesGameAsync(in Streamer streamer, in CancellationToken cancellationToken)
    {
        return this._mediator.Publish(new MarblesStarting(streamer), cancellationToken: cancellationToken);
    }

    private static bool IsHeistMessage(OnMessageReceivedArgs message, TwitchMarbles marbles)
    {
        return StringComparer.InvariantCultureIgnoreCase.Equals(x: marbles.Streamer, y: message.ChatMessage.Channel) &&
               StringComparer.InvariantCultureIgnoreCase.Equals(x: marbles.Bot, y: message.ChatMessage.Username) &&
               StringComparer.InvariantCultureIgnoreCase.Equals(x: marbles.Match, y: message.ChatMessage.Message);
    }

    private async Task<bool> JoinHeistAsync(OnMessageReceivedArgs e, CancellationToken cancellationToken)
    {
        if (IsHeistStartingMessage(e))
        {
            await this._mediator.Publish(new StreamLabsHeistStarting(new(e.ChatMessage.Channel)), cancellationToken: cancellationToken);

            return true;
        }

        return false;
    }

    private static bool IsHeistStartingMessage(OnMessageReceivedArgs e)
    {
        return StringComparer.InvariantCulture.Equals(x: e.ChatMessage.Username, y: "streamlabs") &&
               e.ChatMessage.Message.EndsWith(value: " is trying to get a crew together for a treasure hunt! Type !heist <amount> to join.", comparisonType: StringComparison.Ordinal);
    }

    private Task OnNewSubscriberAsync(OnNewSubscriberArgs e, in CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.Channel);

        this._logger.LogInformation($"{streamer}: New Subscriber {e.Subscriber.DisplayName}");

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(streamer);

        if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
        {
            return state.NewSubscriberPaidAsync(Viewer.FromString(e.Subscriber.DisplayName), cancellationToken: cancellationToken);
        }

        return state.NewSubscriberPrimeAsync(Viewer.FromString(e.Subscriber.DisplayName), cancellationToken: cancellationToken);
    }

    private Task OnReSubscribeAsync(OnReSubscriberArgs e, in CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.Channel);
        this._logger.LogInformation($"{streamer}: Resub {e.ReSubscriber.DisplayName} for {e.ReSubscriber.Months}");

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(streamer);

        if (e.ReSubscriber.SubscriptionPlan == SubscriptionPlan.Prime)
        {
            return state.ResubscribePaidAsync(Viewer.FromString(e.ReSubscriber.DisplayName), months: e.ReSubscriber.Months, cancellationToken: cancellationToken);
        }

        return state.ResubscribePrimeAsync(Viewer.FromString(e.ReSubscriber.DisplayName), months: e.ReSubscriber.Months, cancellationToken: cancellationToken);
    }
}