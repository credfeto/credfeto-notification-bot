using System;
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
    private readonly ILogger<TwitchChat> _logger;
    private readonly IMediator _mediator;

    private readonly TwitchBotOptions _options;
    private readonly ITwitchChannelManager _twitchChannelManager;

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
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

        ConnectionCredentials credentials = new(twitchUsername: this._options.Authentication.UserName, twitchOAuth: this._options.Authentication.OAuthToken);

        this._client.Initialize(credentials: credentials, channels: new() { this._options.Authentication.UserName });

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
                  .Where(e => this._options.IsModChannel(Streamer.FromString(e.Channel)))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnNewSubscriberAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnReSubscriberArgs>(addHandler: h => this._client.OnReSubscriber += h, removeHandler: h => this._client.OnReSubscriber -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => this._options.IsModChannel(Streamer.FromString(e.Channel)))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnReSubscriberAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnCommunitySubscriptionArgs>(addHandler: h => this._client.OnCommunitySubscription += h, removeHandler: h => this._client.OnCommunitySubscription -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => this._options.IsModChannel(Streamer.FromString(e.Channel)))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnCommunitySubscriptionAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnGiftedSubscriptionArgs>(addHandler: h => this._client.OnGiftedSubscription += h, removeHandler: h => this._client.OnGiftedSubscription -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => this._options.IsModChannel(Streamer.FromString(e.Channel)))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnGiftedSubscriptionAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnContinuedGiftedSubscriptionArgs>(addHandler: h => this._client.OnContinuedGiftedSubscription += h,
                                                                       removeHandler: h => this._client.OnContinuedGiftedSubscription -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => this._options.IsModChannel(Streamer.FromString(e.Channel)))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnContinuedGiftedSubscriptionAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnPrimePaidSubscriberArgs>(addHandler: h => this._client.OnPrimePaidSubscriber += h, removeHandler: h => this._client.OnPrimePaidSubscriber -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => this._options.IsModChannel(Streamer.FromString(e.Channel)))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnPrimePaidSubscriberAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        this._twitchChatMessageChannel.ReadAllAsync(CancellationToken.None)
            .ToObservable()
            .Delay(TimeSpan.FromSeconds(1))
            .Where(this.IsConnectedToChat)
            .Subscribe(onNext: this.PublishChatMessage);

        this._client.Connect();
        this._connected = true;
    }

    public void JoinChat(Streamer streamer)
    {
        this._client.JoinChannel(streamer.Value);
    }

    public void LeaveChat(Streamer streamer)
    {
        if (StringComparer.InvariantCultureIgnoreCase.Equals(x: this._options.Authentication.UserName, y: streamer.Value))
        {
            // never leave own channel.
            return;
        }

        this._client.LeaveChannel(streamer.Value);
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

    private bool IsConnectedToChat(TwitchChatMessage chatMessage)
    {
        return this._client.JoinedChannels.Any(joinedChannel => StringComparer.InvariantCultureIgnoreCase.Equals(x: chatMessage.Streamer.Value, y: joinedChannel.Channel));
    }

    private void PublishChatMessage(TwitchChatMessage twitchChatMessage)
    {
        try
        {
            this._logger.LogInformation($"{twitchChatMessage.Streamer}: >>> @{this._options.Authentication.UserName} SEND >>> {twitchChatMessage.Message}");

            this._client.SendMessage(channel: twitchChatMessage.Streamer.Value, message: twitchChatMessage.Message);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{twitchChatMessage.Streamer}: Failed to publish message : {exception.Message}");
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

        if (!this._options.IsModChannel(streamer))
        {
            return;
        }

        this._logger.LogInformation($"{streamer}: @{e.ChatMessage.Username}: {e.ChatMessage.Message}");

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(streamer);

        await state.ChatMessageAsync(Viewer.FromString(e.ChatMessage.Username), message: e.ChatMessage.Message, bits: e.ChatMessage.Bits, cancellationToken: cancellationToken);
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

    private Task OnReSubscriberAsync(OnReSubscriberArgs e, in CancellationToken cancellationToken)
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