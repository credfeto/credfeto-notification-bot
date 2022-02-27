using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchChat : ITwitchChat
{
    private readonly TwitchClient _client;
    private readonly IHeistJoiner _heistJoiner;
    private readonly ILogger<TwitchChat> _logger;

    private readonly TwitchBotOptions _options;
    private readonly ITwitchChannelManager _twitchChannelManager;
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;
    private bool _connected;

    public TwitchChat(IOptions<TwitchBotOptions> options,
                      ITwitchChannelManager twitchChannelManager,
                      IMessageChannel<TwitchChatMessage> twitchChatMessageChannel,
                      IHeistJoiner heistJoiner,
                      ILogger<TwitchChat> logger)
    {
        this._twitchChannelManager = twitchChannelManager ?? throw new ArgumentNullException(nameof(twitchChannelManager));
        this._twitchChatMessageChannel = twitchChatMessageChannel;
        this._heistJoiner = heistJoiner ?? throw new ArgumentNullException(nameof(heistJoiner));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

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

        // HEALTH
        Observable.FromEventPattern<OnConnectedArgs>(addHandler: h => this._client.OnConnected += h, removeHandler: h => this._client.OnConnected -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnConnected);

        Observable.FromEventPattern<OnDisconnectedEventArgs>(addHandler: h => this._client.OnDisconnected += h, removeHandler: h => this._client.OnDisconnected -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnDisconnected);

        Observable.FromEventPattern<OnReconnectedEventArgs>(addHandler: h => this._client.OnReconnected += h, removeHandler: h => this._client.OnReconnected -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnReconnected);

        Observable.FromEventPattern<OnJoinedChannelArgs>(addHandler: h => this._client.OnJoinedChannel += h, removeHandler: h => this._client.OnJoinedChannel -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnJoinedChannel);

        // STATE
        Observable.FromEventPattern<OnChannelStateChangedArgs>(addHandler: h => this._client.OnChannelStateChanged += h, removeHandler: h => this._client.OnChannelStateChanged -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnChannelStateChanged);

        // LOGGING
        Observable.FromEventPattern<OnLogArgs>(addHandler: h => this._client.OnLog += h, removeHandler: h => this._client.OnLog -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnLog);

        // CHAT
        Observable.FromEventPattern<OnMessageReceivedArgs>(addHandler: h => this._client.OnMessageReceived += h, removeHandler: h => this._client.OnMessageReceived -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnMessageReceivedAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnChatClearedArgs>(addHandler: h => this._client.OnChatCleared += h, removeHandler: h => this._client.OnChatCleared -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnChatCleared);

        // RAID HOST
        Observable.FromEventPattern<OnRaidNotificationArgs>(addHandler: h => this._client.OnRaidNotification += h, removeHandler: h => this._client.OnRaidNotification -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Where(e => this._options.IsModChannel(e.Channel))
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnRaidAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnBeingHostedArgs>(addHandler: h => this._client.OnBeingHosted += h, removeHandler: h => this._client.OnBeingHosted -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnBeingHosted);

        // SUBS
        Observable.FromEventPattern<OnNewSubscriberArgs>(addHandler: h => this._client.OnNewSubscriber += h, removeHandler: h => this._client.OnNewSubscriber -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnNewSubscriber);

        Observable.FromEventPattern<OnReSubscriberArgs>(addHandler: h => this._client.OnReSubscriber += h, removeHandler: h => this._client.OnReSubscriber -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnReSubscriber);

        Observable.FromEventPattern<OnCommunitySubscriptionArgs>(addHandler: h => this._client.OnCommunitySubscription += h, removeHandler: h => this._client.OnCommunitySubscription -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnCommunitySubscription);

        Observable.FromEventPattern<OnGiftedSubscriptionArgs>(addHandler: h => this._client.OnGiftedSubscription += h, removeHandler: h => this._client.OnGiftedSubscription -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnGiftedSubscription);

        Observable.FromEventPattern<OnContinuedGiftedSubscriptionArgs>(addHandler: h => this._client.OnContinuedGiftedSubscription += h,
                                                                       removeHandler: h => this._client.OnContinuedGiftedSubscription -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnContinuedGiftedSubscription);

        Observable.FromEventPattern<OnPrimePaidSubscriberArgs>(addHandler: h => this._client.OnPrimePaidSubscriber += h, removeHandler: h => this._client.OnPrimePaidSubscriber -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnPrimePaidSubscriber);

        this._twitchChatMessageChannel.ReadAllAsync(CancellationToken.None)
            .ToObservable()
            .Subscribe(this.PublishChatMessage);

        this._client.Connect();
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

        //return Task.CompletedTask;
    }

    private void PublishChatMessage(TwitchChatMessage twitchChatMessage)
    {
        try
        {
            this._client.SendMessage(channel: twitchChatMessage.Channel, message: twitchChatMessage.Message);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{twitchChatMessage.Channel}: Failed to publish message : {exception.Message}");
        }
    }

    private void Client_OnBeingHosted(OnBeingHostedArgs e)
    {
        if (!this._options.Heists.Any(c => StringComparer.InvariantCultureIgnoreCase.Equals(x: c, y: e.BeingHostedNotification.Channel)))
        {
            return;
        }

        this._logger.LogInformation(
            $"{e.BeingHostedNotification.Channel}: Being hosted by {e.BeingHostedNotification.HostedByChannel} Viewers: {e.BeingHostedNotification.Viewers}, AutoHost: {e.BeingHostedNotification.IsAutoHosted}");
    }

    private void Client_OnChatCleared(OnChatClearedArgs e)
    {
        if (!this._options.IsModChannel(e.Channel))
        {
            return;
        }

        this._logger.LogInformation($"{e.Channel}: Chat Cleared");

        TwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        state.ClearChat();
    }

    private void Client_OnCommunitySubscription(OnCommunitySubscriptionArgs e)
    {
        if (!this._options.IsModChannel(e.Channel))
        {
            return;
        }

        this._logger.LogInformation($"{e.Channel}: Community Sub: {e.GiftedSubscription.DisplayName}");

        if (e.GiftedSubscription.IsAnonymous)
        {
            return;
        }

        TwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        state.GiftedMultiple(giftedBy: e.GiftedSubscription.DisplayName, count: e.GiftedSubscription.MsgParamMassGiftCount, months: e.GiftedSubscription.MsgParamMultiMonthGiftDuration);
    }

    private void Client_OnGiftedSubscription(OnGiftedSubscriptionArgs e)
    {
        if (!this._options.IsModChannel(e.Channel))
        {
            return;
        }

        this._logger.LogInformation($"{e.Channel}: Community Sub: {e.GiftedSubscription.DisplayName}");

        if (e.GiftedSubscription.IsAnonymous)
        {
            return;
        }

        TwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        state.GiftedSub(giftedBy: e.GiftedSubscription.DisplayName, months: e.GiftedSubscription.MsgParamMultiMonthGiftDuration);
    }

    private void Client_OnChannelStateChanged(OnChannelStateChangedArgs e)
    {
        if (!this._options.IsModChannel(e.Channel))
        {
            return;
        }

        this._logger.LogInformation($"{e.Channel}: Emote Only: {e.ChannelState.EmoteOnly} Follower Only: {e.ChannelState.FollowersOnly} Sub Only: {e.ChannelState.SubOnly}");
    }

    private void Client_OnContinuedGiftedSubscription(OnContinuedGiftedSubscriptionArgs e)
    {
        if (!this._options.IsModChannel(e.Channel))
        {
            return;
        }

        this._logger.LogInformation($"{e.Channel}: {e.ContinuedGiftedSubscription.DisplayName} continued sub gifted by {e.ContinuedGiftedSubscription.MsgParamSenderLogin}");

        TwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        state.ContinuedSub(e.ContinuedGiftedSubscription.DisplayName);
    }

    private void Client_OnPrimePaidSubscriber(OnPrimePaidSubscriberArgs e)
    {
        if (!this._options.IsModChannel(e.Channel))
        {
            return;
        }

        this._logger.LogInformation($"{e.Channel}: {e.PrimePaidSubscriber.DisplayName} converted prime sub to paid");

        TwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        state.PrimeToPaid(e.PrimePaidSubscriber.DisplayName);
    }

    private void Client_OnLog(OnLogArgs e)
    {
        this._logger.LogDebug($"{e.DateTime}: {e.BotUsername} - {e.Data}");
    }

    private void Client_OnConnected(OnConnectedArgs e)
    {
        this._logger.LogWarning($"Connected to {e.BotUsername} {e.AutoJoinChannel}");
        this._connected = true;
    }

    private void Client_OnDisconnected(OnDisconnectedEventArgs e)
    {
        this._logger.LogWarning("Disconnected :(");
        this._connected = false;
    }

    private void Client_OnReconnected(OnReconnectedEventArgs e)
    {
        this._logger.LogWarning("Reconnected :)");
        this._connected = true;
    }

    private Task OnRaidAsync(OnRaidNotificationArgs e, in CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{e.Channel}: Raided by {e.RaidNotification.DisplayName}");

        TwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        return state.RaidedAsync(raider: e.RaidNotification.DisplayName, viewerCount: e.RaidNotification.MsgParamViewerCount, cancellationToken: cancellationToken);
    }

    private void Client_OnJoinedChannel(OnJoinedChannelArgs e)
    {
        this._logger.LogInformation($"{e.Channel}: Joining channel as {e.BotUsername}");
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
            await this.JoinHeistAsync(e: e, cancellationToken: cancellationToken);
        }

        TwitchChannelState state = this._twitchChannelManager.GetChannel(e.ChatMessage.Channel);

        await state.ChatMessageAsync(user: e.ChatMessage.Username, message: e.ChatMessage.Message, bits: e.ChatMessage.Bits, cancellationToken: cancellationToken);
    }

    private async Task JoinHeistAsync(OnMessageReceivedArgs e, CancellationToken cancellationToken)
    {
        //:streamlabs!streamlabs@streamlabs.tmi.twitch.tv PRIVMSG #emilyisfun :Ahoy! Captain reckless_fury is trying to get a crew together for a treasure hunt! Type !heist <amount> to join.
        if (StringComparer.InvariantCulture.Equals(x: e.ChatMessage.Username, y: "streamlabs") && e.ChatMessage.Message.StartsWith(value: "Ahoy! Captain ", comparisonType: StringComparison.Ordinal) &&
            e.ChatMessage.Message.EndsWith(value: " is trying to get a crew together for a treasure hunt! Type !heist <amount> to join.", comparisonType: StringComparison.Ordinal))
        {
            this._logger.LogInformation($"{e.ChatMessage.Channel}: Heist Starting!");
            await this._heistJoiner.JoinHeistAsync(channel: e.ChatMessage.Channel, cancellationToken: cancellationToken);
        }
    }

    private void Client_OnNewSubscriber(OnNewSubscriberArgs e)
    {
        if (!this._options.IsModChannel(e.Channel))
        {
            return;
        }

        this._logger.LogInformation($"{e.Channel}: New Subscriber {e.Subscriber.DisplayName}");

        TwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
        {
            state.NewSubscriberPaid(e.Subscriber.DisplayName);
        }
        else
        {
            state.NewSubscriberPrime(e.Subscriber.DisplayName);
        }
    }

    private void Client_OnReSubscriber(OnReSubscriberArgs e)
    {
        if (!this._options.IsModChannel(e.Channel))
        {
            return;
        }

        this._logger.LogInformation($"{e.Channel}: Resub {e.ReSubscriber.DisplayName} for {e.ReSubscriber.Months}");

        TwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        if (e.ReSubscriber.SubscriptionPlan == SubscriptionPlan.Prime)
        {
            state.ResubscribePaid(user: e.ReSubscriber.DisplayName, months: e.ReSubscriber.Months);
        }
        else
        {
            state.ResubscribePrime(user: e.ReSubscriber.DisplayName, months: e.ReSubscriber.Months);
        }
    }
}