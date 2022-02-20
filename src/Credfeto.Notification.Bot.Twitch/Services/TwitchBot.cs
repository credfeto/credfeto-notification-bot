using System;
using System.Collections.Generic;
using System.Linq;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchBot : ITwitchBot
{
    private readonly TwitchClient _client;
    private readonly ILogger<TwitchBot> _logger;
    private readonly TwitchBotOptions _options;

    public TwitchBot(IOptions<TwitchBotOptions> options, ILogger<TwitchBot> logger)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

        ConnectionCredentials credentials = new(twitchUsername: this._options.Authentication.UserName, twitchOAuth: this._options.Authentication.OAuthToken);
        ClientOptions clientOptions = new() { MessagesAllowedInPeriod = 750, ThrottlingPeriod = TimeSpan.FromSeconds(30) };
        WebSocketClient customClient = new(clientOptions);
        this._client = new(customClient);
        List<string> channels = new[]
                                {
                                    this._options.Authentication.UserName
                                }.Concat(this._options.Channels)
                                 .Select(c => c.ToLowerInvariant())
                                 .Distinct()
                                 .ToList();

        this._client.Initialize(credentials: credentials, channels: channels);

        this._client.OnLog += this.Client_OnLog;
        this._client.OnJoinedChannel += this.Client_OnJoinedChannel;
        this._client.OnMessageReceived += this.Client_OnMessageReceived;
        this._client.OnWhisperReceived += this.Client_OnWhisperReceived;
        this._client.OnNewSubscriber += this.Client_OnNewSubscriber;
        this._client.OnConnected += this.Client_OnConnected;

        this._client.Connect();
    }

    private void Client_OnLog(object? sender, OnLogArgs e)
    {
        this._logger.LogInformation($"{e.DateTime}: {e.BotUsername} - {e.Data}");
    }

    private void Client_OnConnected(object? sender, OnConnectedArgs e)
    {
        this._logger.LogInformation($"Connected to {e.AutoJoinChannel}");
    }

    private void Client_OnJoinedChannel(object? sender, OnJoinedChannelArgs e)
    {
        this._logger.LogInformation("Hey guys! I am a bot connected via TwitchLib!");

        //this._client.SendMessage(channel: e.Channel, message: "Hey guys! I am a bot connected via TwitchLib!");
    }

    private void Client_OnMessageReceived(object? sender, OnMessageReceivedArgs e)
    {
        if (e.ChatMessage.Username == "credfeto")
        {
            this._client.SendReply(channel: e.ChatMessage.Channel, replyToId: e.ChatMessage.Username, $"Hello @{e.ChatMessage.Username}");
        }

        // if (e.ChatMessage.Message.Contains("badword"))
        // {
        //     this._client.TimeoutUser(channel: e.ChatMessage.Channel, viewer: e.ChatMessage.Username, TimeSpan.FromMinutes(30), message: "Bad word! 30 minute timeout!");
        // }
    }

    private void Client_OnWhisperReceived(object? sender, OnWhisperReceivedArgs e)
    {
        if (e.WhisperMessage.Username == "my_friend")
        {
            this._client.SendWhisper(receiver: e.WhisperMessage.Username, message: "Hey! Whispers are so cool!!");
        }
    }

    private void Client_OnNewSubscriber(object? sender, OnNewSubscriberArgs e)
    {
        if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
        {
            this._client.SendMessage(channel: e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
        }
        else
        {
            this._client.SendMessage(channel: e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
        }
    }
}