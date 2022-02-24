using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwitchLib.Api;
using TwitchLib.Api.Interfaces;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchBot : ITwitchBot
{
    private readonly ITwitchAPI _api;
    private readonly TwitchClient _client;
    private readonly ICurrentTimeSource _currentTimeSource;
    private readonly ILogger<TwitchBot> _logger;

    private readonly LiveStreamMonitorService _lsm;
    private readonly TwitchBotOptions _options;

    public TwitchBot(IOptions<TwitchBotOptions> options, ICurrentTimeSource currentTimeSource, ILogger<TwitchBot> logger)
    {
        this._currentTimeSource = currentTimeSource;
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

        List<string> channels = new[]
                                {
                                    this._options.Authentication.UserName
                                }.Concat(this._options.Channels)
                                 .Select(c => c.ToLowerInvariant())
                                 .Distinct()
                                 .ToList();

        this._api = new TwitchAPI();
        this._api.Settings.ClientId = "TODO";
        this._api.Settings.AccessToken = "TODO";
        this._lsm = new(this._api);
        this._lsm.SetChannelsByName(channels.Concat(new[]
                                                    {
                                                        "steveforward"
                                                    })
                                            .ToList());

        ConnectionCredentials credentials = new(twitchUsername: this._options.Authentication.UserName, twitchOAuth: this._options.Authentication.OAuthToken);
        ClientOptions clientOptions = new() { MessagesAllowedInPeriod = 750, ThrottlingPeriod = TimeSpan.FromSeconds(30) };
        WebSocketClient customClient = new(clientOptions);
        TwitchClient client = new(customClient) { OverrideBeingHostedCheck = true };

        client.Initialize(credentials: credentials, channels: channels);
        this._client = client;

        Observable.FromEventPattern<OnLogArgs>(addHandler: h => this._client.OnLog += h, removeHandler: h => this._client.OnLog -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnLog);

        Observable.FromEventPattern<OnJoinedChannelArgs>(addHandler: h => this._client.OnJoinedChannel += h, removeHandler: h => this._client.OnJoinedChannel -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnJoinedChannel);

        Observable.FromEventPattern<OnMessageReceivedArgs>(addHandler: h => this._client.OnMessageReceived += h, removeHandler: h => this._client.OnMessageReceived -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnMessageReceived);

        Observable.FromEventPattern<OnWhisperReceivedArgs>(addHandler: h => this._client.OnWhisperReceived += h, removeHandler: h => this._client.OnWhisperReceived -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnWhisperReceived);

        Observable.FromEventPattern<OnNewSubscriberArgs>(addHandler: h => this._client.OnNewSubscriber += h, removeHandler: h => this._client.OnNewSubscriber -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnNewSubscriber);

        Observable.FromEventPattern<OnConnectedArgs>(addHandler: h => this._client.OnConnected += h, removeHandler: h => this._client.OnConnected -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnConnected);

        Observable.FromEventPattern<OnRaidNotificationArgs>(addHandler: h => this._client.OnRaidNotification += h, removeHandler: h => this._client.OnRaidNotification -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnRaided);

        Observable.FromEventPattern<OnBeingHostedArgs>(addHandler: h => this._client.OnBeingHosted += h, removeHandler: h => this._client.OnBeingHosted -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnBeingHosted);

        Observable.FromEventPattern<OnChatClearedArgs>(addHandler: h => this._client.OnChatCleared += h, removeHandler: h => this._client.OnChatCleared -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnChatCleared);

        Observable.FromEventPattern<OnCommunitySubscriptionArgs>(addHandler: h => this._client.OnCommunitySubscription += h, removeHandler: h => this._client.OnCommunitySubscription -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnCommunitySubscription);

        Observable.FromEventPattern<OnGiftedSubscriptionArgs>(addHandler: h => this._client.OnGiftedSubscription += h, removeHandler: h => this._client.OnGiftedSubscription -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnGiftedSubscription);

        Observable.FromEventPattern<OnChannelStateChangedArgs>(addHandler: h => this._client.OnChannelStateChanged += h, removeHandler: h => this._client.OnChannelStateChanged -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnChannelStateChanged);

        Observable.FromEventPattern<OnContinuedGiftedSubscriptionArgs>(addHandler: h => this._client.OnContinuedGiftedSubscription += h,
                                                                       removeHandler: h => this._client.OnContinuedGiftedSubscription -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnContinuedGiftedSubscription);

        Observable.FromEventPattern<OnPrimePaidSubscriberArgs>(addHandler: h => this._client.OnPrimePaidSubscriber += h, removeHandler: h => this._client.OnPrimePaidSubscriber -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnPrimePaidSubscriber);

        Observable.FromEventPattern<OnStreamOnlineArgs>(addHandler: h => this._lsm.OnStreamOnline += h, removeHandler: h => this._lsm.OnStreamOnline -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnStreamOnline);

        Observable.FromEventPattern<OnStreamOfflineArgs>(addHandler: h => this._lsm.OnStreamOffline += h, removeHandler: h => this._lsm.OnStreamOffline -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.Client_OnStreamOffline);

        this._client.Connect();
    }

    public Task UpdateAsync()
    {
        this._logger.LogDebug("Tick...");

        //return this._lsm.UpdateLiveStreamersAsync();
        return Task.CompletedTask;
    }

    private void Client_OnBeingHosted(OnBeingHostedArgs e)
    {
        this._logger.LogInformation(
            $"{e.BeingHostedNotification.Channel}: Being hosted by {e.BeingHostedNotification.HostedByChannel} Viewers: {e.BeingHostedNotification.Viewers}, AutoHost: {e.BeingHostedNotification.IsAutoHosted}");
    }

    private void Client_OnChatCleared(OnChatClearedArgs e)
    {
        this._logger.LogInformation($"{e.Channel}: Chat Cleared");
    }

    private void Client_OnCommunitySubscription(OnCommunitySubscriptionArgs e)
    {
        this._logger.LogInformation($"{e.Channel}: Community Sub: {e.GiftedSubscription.DisplayName}");
    }

    private void Client_OnGiftedSubscription(OnGiftedSubscriptionArgs e)
    {
        this._logger.LogInformation($"{e.Channel}: Community Sub: {e.GiftedSubscription.DisplayName}");
    }

    private void Client_OnChannelStateChanged(OnChannelStateChangedArgs e)
    {
        this._logger.LogInformation($"{e.Channel}: Emote Only: {e.ChannelState.EmoteOnly} Follower Only {e.ChannelState.FollowersOnly} Sub Only {e.ChannelState.SubOnly}");
    }

    private void Client_OnContinuedGiftedSubscription(OnContinuedGiftedSubscriptionArgs e)
    {
        this._logger.LogInformation($"{e.Channel}: {e.ContinuedGiftedSubscription.DisplayName} continued sub gifted by {e.ContinuedGiftedSubscription.MsgParamSenderLogin}");
    }

    private void Client_OnPrimePaidSubscriber(OnPrimePaidSubscriberArgs e)
    {
        this._logger.LogInformation($"{e.Channel}: {e.PrimePaidSubscriber.DisplayName} converted prime sub to paid {e.PrimePaidSubscriber}");
    }

    private void Client_OnStreamOnline(OnStreamOnlineArgs e)
    {
        this._logger.LogInformation($"{e.Channel}: Started streaming {e.Stream.Title} ({e.Stream.GameName}");
    }

    private void Client_OnStreamOffline(OnStreamOfflineArgs e)
    {
        this._logger.LogInformation($"{e.Channel}: Stopped streaming {e.Stream.Title} ({e.Stream.GameName}");
    }

    private void Client_OnLog(OnLogArgs e)
    {
        this._logger.LogInformation($"{e.DateTime}: {e.BotUsername} - {e.Data}");
    }

    private void Client_OnConnected(OnConnectedArgs e)
    {
        this._logger.LogInformation($"Connected to {e.AutoJoinChannel} by {e.BotUsername}");
    }

    private void Client_OnRaided(OnRaidNotificationArgs e)
    {
        this._logger.LogInformation($"Raided by {e.RaidNotification.DisplayName}");

        if (this._options.Raids.Contains(e.Channel))
        {
            const string raidWelcome = @"
♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫
GlitchLit  GlitchLit  GlitchLit Welcome raiders! GlitchLit GlitchLit GlitchLit
♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫";

            this._client.SendMessage(channel: e.Channel, message: raidWelcome);
            this._client.SendMessage(channel: e.Channel, $"Thanks @{e.RaidNotification.DisplayName} for the raid");
            this._client.SendMessage(channel: e.Channel, $"Check out https://www.twitch.tv/{e.RaidNotification.DisplayName}");
        }
    }

    private void Client_OnJoinedChannel(OnJoinedChannelArgs e)
    {
        this._logger.LogInformation($"{e.Channel} Joining channel as {e.BotUsername}");

        //this._client.SendMessage(channel: e.Channel, message: "Hey guys! I am a bot connected via TwitchLib!");
    }

    private void Client_OnMessageReceived(OnMessageReceivedArgs e)
    {
        this._logger.LogInformation($"{e.ChatMessage.Channel}: @{e.ChatMessage.Username}: {e.ChatMessage.Message}");

        if (this._options.Heists.Contains(e.ChatMessage.Channel))
        {
            //:streamlabs!streamlabs@streamlabs.tmi.twitch.tv PRIVMSG #emilyisfun :Ahoy! Captain reckless_fury is trying to get a crew together for a treasure hunt! Type !heist <amount> to join.

            if (StringComparer.InvariantCulture.Equals(x: e.ChatMessage.Username, y: "streamlabs") &&
                e.ChatMessage.Message.StartsWith(value: "Ahoy! Captain ", comparisonType: StringComparison.Ordinal) &&
                e.ChatMessage.Message.EndsWith(value: " is trying to get a crew together for a treasure hunt! Type !heist <amount> to join.", comparisonType: StringComparison.Ordinal))
            {
                this._logger.LogInformation($"{e.ChatMessage.Channel}: Heist Starting!");

                //this._client.SendMessage(channel: e.ChatMessage.Channel, message: "!heist all");
            }
        }

        // if (e.ChatMessage.Username.ToLowerInvariant() is "credfeto" or "steveforward")
        // {
        //     this._client.SendReply(channel: e.ChatMessage.Channel, replyToId: e.ChatMessage.Username, $"Hello @{e.ChatMessage.Username} it's {this._currentTimeSource.UtcNow()}");
        // }

        // if (e.ChatMessage.Message.Contains("badword"))
        // {
        //     this._client.TimeoutUser(channel: e.ChatMessage.Channel, viewer: e.ChatMessage.Username, TimeSpan.FromMinutes(30), message: "Bad word! 30 minute timeout!");
        // }
    }

    private void Client_OnWhisperReceived(OnWhisperReceivedArgs e)
    {
        // if (e.WhisperMessage.Username == "my_friend")
        // {
        //     this._client.SendWhisper(receiver: e.WhisperMessage.Username, message: "Hey! Whispers are so cool!!");
        // }
    }

    private void Client_OnNewSubscriber(OnNewSubscriberArgs e)
    {
        this._logger.LogInformation($"{e.Channel}: New Subscriber {e.Subscriber.DisplayName}");

        // if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
        // {
        //     this._client.SendMessage(channel: e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
        // }
        // else
        // {
        //     this._client.SendMessage(channel: e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
        // }
    }
}