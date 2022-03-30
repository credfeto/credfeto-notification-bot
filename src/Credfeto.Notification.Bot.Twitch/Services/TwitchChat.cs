using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.StreamState;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NonBlocking;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;
using OnLogArgs = TwitchLib.Client.Events.OnLogArgs;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchChat : ITwitchChat
{
    private readonly TwitchClient _client;
    private readonly ILogger<TwitchChat> _logger;
    private readonly IMediator _mediator;

    private readonly TwitchBotOptions _options;
    private readonly TwitchPubSub _pubSub;
    private readonly ITwitchChannelManager _twitchChannelManager;

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;
    private readonly IUserInfoService _userInfoService;
    private readonly ConcurrentDictionary<string, string> _userMappings;
    private bool _connected;

    public TwitchChat(IOptions<TwitchBotOptions> options,
                      IUserInfoService userInfoService,
                      ITwitchChannelManager twitchChannelManager,
                      IMessageChannel<TwitchChatMessage> twitchChatMessageChannel,
                      IMediator mediator,
                      ILogger<TwitchChat> logger)
    {
        this._userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        this._twitchChannelManager = twitchChannelManager ?? throw new ArgumentNullException(nameof(twitchChannelManager));
        this._twitchChatMessageChannel = twitchChatMessageChannel;
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

        this._pubSub = new();

        this._userMappings = new(StringComparer.InvariantCultureIgnoreCase);

        List<string> channels = new[]
                                {
                                    this._options.Authentication.UserName
                                }.Concat(this._options.Channels)
                                 .Concat(this._options.Heists)
                                 .Select(c => c.ToLowerInvariant())
                                 .Distinct()
                                 .ToList();

        ConnectionCredentials credentials = new(twitchUsername: this._options.Authentication.UserName, twitchOAuth: this._options.Authentication.OAuthToken);
        TwitchClient client = new(new WebSocketClient(new ClientOptions
                                                      {
                                                          MessagesAllowedInPeriod = 750,
                                                          ThrottlingPeriod = TimeSpan.FromSeconds(30),
                                                          ReconnectionPolicy = new(reconnectInterval: 1000, maxAttempts: null)
                                                      })) { OverrideBeingHostedCheck = true, AutoReListenOnException = false };

        client.Initialize(credentials: credentials, channels: channels);
        this._client = client;

        // FOLLOWS

        Observable.FromEventPattern<OnPubSubServiceErrorArgs>(addHandler: h => this._pubSub.OnPubSubServiceError += h, removeHandler: h => this._pubSub.OnPubSubServiceError -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.OnPubSubServiceError);

        Observable.FromEventPattern(addHandler: h => this._pubSub.OnPubSubServiceConnected += h, removeHandler: h => this._pubSub.OnPubSubServiceConnected -= h)
                  .Subscribe(this.OnPubSubServiceConnected);

        Observable.FromEventPattern<OnFollowArgs>(addHandler: h => this._pubSub.OnFollow += h, removeHandler: h => this._pubSub.OnFollow -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnFollowedAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

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
                  .Where(e => this._options.IsModChannel(e.Channel))
                  .Subscribe(onNext: this.Client_OnChatCleared);

        // RAIDS
        Observable.FromEventPattern<OnRaidNotificationArgs>(addHandler: h => this._client.OnRaidNotification += h, removeHandler: h => this._client.OnRaidNotification -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => this._options.IsModChannel(e.Channel))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnRaidAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        // SUBS
        Observable.FromEventPattern<OnNewSubscriberArgs>(addHandler: h => this._client.OnNewSubscriber += h, removeHandler: h => this._client.OnNewSubscriber -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => this._options.IsModChannel(e.Channel))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnNewSubscriberAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnReSubscriberArgs>(addHandler: h => this._client.OnReSubscriber += h, removeHandler: h => this._client.OnReSubscriber -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => this._options.IsModChannel(e.Channel))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnReSubscriberAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnCommunitySubscriptionArgs>(addHandler: h => this._client.OnCommunitySubscription += h, removeHandler: h => this._client.OnCommunitySubscription -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => this._options.IsModChannel(e.Channel))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnCommunitySubscriptionAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnGiftedSubscriptionArgs>(addHandler: h => this._client.OnGiftedSubscription += h, removeHandler: h => this._client.OnGiftedSubscription -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => this._options.IsModChannel(e.Channel))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnGiftedSubscriptionAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnContinuedGiftedSubscriptionArgs>(addHandler: h => this._client.OnContinuedGiftedSubscription += h,
                                                                       removeHandler: h => this._client.OnContinuedGiftedSubscription -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => this._options.IsModChannel(e.Channel))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnContinuedGiftedSubscriptionAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnPrimePaidSubscriberArgs>(addHandler: h => this._client.OnPrimePaidSubscriber += h, removeHandler: h => this._client.OnPrimePaidSubscriber -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => this._options.IsModChannel(e.Channel))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnPrimePaidSubscriberAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        this._twitchChatMessageChannel.ReadAllAsync(CancellationToken.None)
            .ToObservable()
            .Subscribe(onNext: this.PublishChatMessage);

        this._client.Connect();
        this._pubSub.Connect();
        this._connected = true;
    }

    public Task UpdateAsync()
    {
        if (!this._connected)
        {
            this._logger.LogDebug("Reconnecting...");

            //this._client.Connect();
            this._connected = true;
        }

        return Task.CompletedTask;
    }

    private void PublishChatMessage(TwitchChatMessage twitchChatMessage)
    {
        try
        {
            this._logger.LogInformation($"{twitchChatMessage.Channel}: >>> @{this._options.Authentication.UserName} SEND >>> {twitchChatMessage.Message}");
            this._client.SendMessage(channel: twitchChatMessage.Channel, message: twitchChatMessage.Message);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{twitchChatMessage.Channel}: Failed to publish message : {exception.Message}");
        }
    }

    private void Client_OnChatCleared(OnChatClearedArgs e)
    {
        if (!this._options.IsModChannel(e.Channel))
        {
            return;
        }

        this._logger.LogInformation($"{e.Channel}: Chat Cleared");

        ITwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        state.ClearChat();
    }

    private Task OnCommunitySubscriptionAsync(OnCommunitySubscriptionArgs e, in CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{e.Channel}: Community Sub: {e.GiftedSubscription.DisplayName}");

        if (e.GiftedSubscription.IsAnonymous)
        {
            return Task.CompletedTask;
        }

        ITwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        return state.GiftedMultipleAsync(giftedBy: e.GiftedSubscription.DisplayName,
                                         count: e.GiftedSubscription.MsgParamMassGiftCount,
                                         months: e.GiftedSubscription.MsgParamMultiMonthGiftDuration,
                                         cancellationToken: cancellationToken);
    }

    private Task OnGiftedSubscriptionAsync(OnGiftedSubscriptionArgs e, in CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{e.Channel}: Community Sub: {e.GiftedSubscription.DisplayName}");

        if (e.GiftedSubscription.IsAnonymous)
        {
            return Task.CompletedTask;
        }

        ITwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        return state.GiftedSubAsync(giftedBy: e.GiftedSubscription.DisplayName, months: e.GiftedSubscription.MsgParamMultiMonthGiftDuration, cancellationToken: cancellationToken);
    }

    private void Client_OnChannelStateChanged(OnChannelStateChangedArgs e)
    {
        if (!this._options.IsModChannel(e.Channel))
        {
            return;
        }

        this._logger.LogInformation($"{e.Channel}: Emote Only: {e.ChannelState.EmoteOnly} Follower Only: {e.ChannelState.FollowersOnly} Sub Only: {e.ChannelState.SubOnly}");
    }

    private Task OnContinuedGiftedSubscriptionAsync(OnContinuedGiftedSubscriptionArgs e, in CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{e.Channel}: {e.ContinuedGiftedSubscription.DisplayName} continued sub gifted by {e.ContinuedGiftedSubscription.MsgParamSenderLogin}");

        ITwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        return state.ContinuedSubAsync(user: e.ContinuedGiftedSubscription.DisplayName, cancellationToken: cancellationToken);
    }

    private Task OnPrimePaidSubscriberAsync(OnPrimePaidSubscriberArgs e, in CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{e.Channel}: {e.PrimePaidSubscriber.DisplayName} converted prime sub to paid");

        ITwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        return state.PrimeToPaidAsync(user: e.PrimePaidSubscriber.DisplayName, cancellationToken: cancellationToken);
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
        this._logger.LogWarning("Disconnected :(");
        this._connected = false;
    }

    private void OnReconnected(OnReconnectedEventArgs e)
    {
        this._logger.LogWarning("Reconnected :)");
        this._connected = true;
    }

    private Task OnRaidAsync(OnRaidNotificationArgs e, in CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{e.Channel}: Raided by {e.RaidNotification.DisplayName}");

        ITwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        if (!int.TryParse(s: e.RaidNotification.MsgParamViewerCount, style: NumberStyles.Integer, provider: CultureInfo.InvariantCulture, out int viewerCount))
        {
            viewerCount = 1;
        }

        return state.RaidedAsync(raider: e.RaidNotification.DisplayName, viewerCount: viewerCount, cancellationToken: cancellationToken);
    }

    private Task OnFollowedAsync(OnFollowArgs e, in CancellationToken cancellationToken)
    {
        if (!this._userMappings.TryGetValue(key: e.FollowedChannelId, out string? channelName))
        {
            return Task.CompletedTask;
        }

        this._logger.LogInformation($"{channelName}: (Id: {e.FollowedChannelId}) Followed by {e.Username}");

        if (!this._options.IsModChannel(channelName))
        {
            return Task.CompletedTask;
        }

        ITwitchChannelState state = this._twitchChannelManager.GetChannel(channelName);

        return state.NewFollowerAsync(username: e.Username, cancellationToken: cancellationToken);
    }

    private async Task OnJoinedChannelAsync(OnJoinedChannelArgs e, CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{e.Channel}: Joining channel as {e.BotUsername}");

        if (!this._options.IsModChannel(e.Channel))
        {
            return;
        }

        try
        {
            // TODO: Consider moving PubSub to own class
            TwitchUser? channel = await this._userInfoService.GetUserAsync(e.Channel);

            if (channel != null)
            {
                this._logger.LogInformation($"{e.Channel}: Listening for new follows as {channel.Id} using pubsub");
                this._pubSub.SendTopics();
                this._pubSub.ListenToFollows(channel.Id);
                this._userMappings.GetOrAdd(key: channel.Id, value: channel.UserName);

                await this._mediator.Publish(new TwitchChannelChatConnected(channel.UserName), cancellationToken: cancellationToken);
            }
        }
        catch (Exception exception)
        {
            this._logger.LogError($"{e.Channel}: Failed to initialise: {exception.Message}");
        }
    }

    private async Task OnMessageReceivedAsync(OnMessageReceivedArgs e, CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{e.ChatMessage.Channel}: @{e.ChatMessage.Username}: {e.ChatMessage.Message}");

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

        ITwitchChannelState state = this._twitchChannelManager.GetChannel(e.ChatMessage.Channel);

        await state.ChatMessageAsync(user: e.ChatMessage.Username, message: e.ChatMessage.Message, bits: e.ChatMessage.Bits, cancellationToken: cancellationToken);
    }

    private async Task<bool> JoinHeistAsync(OnMessageReceivedArgs e, CancellationToken cancellationToken)
    {
        if (IsHeistStartingMessage(e))
        {
            await this._mediator.Publish(new StreamLabsHeistStarting(e.ChatMessage.Channel), cancellationToken: cancellationToken);

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
        this._logger.LogInformation($"{e.Channel}: New Subscriber {e.Subscriber.DisplayName}");

        ITwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
        {
            return state.NewSubscriberPaidAsync(user: e.Subscriber.DisplayName, cancellationToken: cancellationToken);
        }

        return state.NewSubscriberPrimeAsync(user: e.Subscriber.DisplayName, cancellationToken: cancellationToken);
    }

    private Task OnReSubscriberAsync(OnReSubscriberArgs e, in CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{e.Channel}: Resub {e.ReSubscriber.DisplayName} for {e.ReSubscriber.Months}");

        ITwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        if (e.ReSubscriber.SubscriptionPlan == SubscriptionPlan.Prime)
        {
            return state.ResubscribePaidAsync(user: e.ReSubscriber.DisplayName, months: e.ReSubscriber.Months, cancellationToken: cancellationToken);
        }

        return state.ResubscribePrimeAsync(user: e.ReSubscriber.DisplayName, months: e.ReSubscriber.Months, cancellationToken: cancellationToken);
    }

    private void OnPubSubServiceError(OnPubSubServiceErrorArgs e)
    {
        this._logger.LogError($"{e.Exception.Message}");
    }

    private void OnPubSubServiceConnected(EventPattern<object> e)
    {
        this._logger.LogInformation("PubSub Connected");
    }
}