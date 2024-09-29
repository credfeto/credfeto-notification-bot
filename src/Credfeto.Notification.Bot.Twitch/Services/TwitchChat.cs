using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.StreamState;
using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NonBlocking;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchChat : ITwitchChat, IDisposable
{
    private readonly IDisposable _chatConnected;
    private readonly IDisposable _chatDisconnected;
    private readonly IDisposable? _chatLogMessage;
    private readonly IDisposable _chatMessageReceived;
    private readonly IDisposable _chatReconnected;
    private readonly TwitchClient _client;

    private readonly ConcurrentDictionary<Streamer, bool> _joinedStreamers;
    private readonly ConcurrentDictionary<Streamer, string> _lastMessage;
    private readonly SemaphoreSlim _lastMessageLock;
    private readonly ILogger<TwitchChat> _logger;
    private readonly IMediator _mediator;

    private readonly TwitchBotOptions _options;
    private readonly IDisposable _sentChatMessages;

    [SuppressMessage(category: "ReSharper", checkId: "PrivateFieldCanBeConvertedToLocalVariable", Justification = "TODO: Review")]
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    private bool _connected;

    public TwitchChat(IOptions<TwitchBotOptions> options, IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, IMediator mediator, ITwitchClient twitchClient, ILogger<TwitchChat> logger)
    {
        this._twitchChatMessageChannel = twitchChatMessageChannel;
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        this._client = twitchClient as TwitchClient ?? throw new ArgumentNullException(nameof(twitchClient));

        this._lastMessageLock = new(initialCount: 1, maxCount: 1);
        this._joinedStreamers = new();
        this._lastMessage = new();

        ConnectionCredentials credentials = new(twitchUsername: this._options.Authentication.UserName, twitchOAuth: this._options.Authentication.OAuthToken);

        this._client.Initialize(credentials: credentials, [this._options.Authentication.UserName]);

        this._joinedStreamers.TryAdd(Streamer.FromString(this._options.Authentication.UserName), value: true);

        // HEALTH
        this._chatConnected = this.SubscribeToChatConnection();
        this._chatDisconnected = this.SubscribeToChatDisconnection();
        this._chatReconnected = this.SubscribeToChatReconnections();

        // LOGGING
        this._chatLogMessage = this._logger.IsEnabled(LogLevel.Debug)
            ? this.SubscribeToChatLogMessages()
            : null;

        // CHAT
        this._chatMessageReceived = this.SubscribeToIncomingChatMessages();

        // MESSAGES BEING SENT
        this._sentChatMessages = this.SubscribeToOutgoingChatMessages();

        this._client.Connect();
        this._connected = true;

        this._client.JoinChannel(this._options.Authentication.UserName);
    }

    public void Dispose()
    {
        this._chatConnected.Dispose();
        this._chatDisconnected.Dispose();
        this._chatLogMessage?.Dispose();
        this._chatMessageReceived.Dispose();
        this._chatReconnected.Dispose();
        this._lastMessageLock.Dispose();
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
        MessagePriority priority = twitchChatMessage.Priority;

        double delay = Jitter.WithJitter((int)priority, GetMaxSeconds(priority));

        this._logger.LogInformation($"{twitchChatMessage.Streamer}: Delaying message for {delay} seconds for: {twitchChatMessage.Message}");

        return TimeSpan.FromSeconds(delay);

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
                if (!twitchChatMessage.Message.StartsWith('!') && !twitchChatMessage.Message.StartsWith('/'))
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
        IReadOnlyList<Streamer> streamers = this.GetPreviouslyJoinedStreamers();

        foreach (Streamer streamer in streamers)
        {
            this._logger.LogInformation($"{streamer}: Reconnecting to chat...");
            this.JoinChat(streamer);
        }
    }

    private IReadOnlyList<Streamer> GetPreviouslyJoinedStreamers()
    {
        return this._joinedStreamers.Keys.Where(streamer => !this.IsConnectedToChat(streamer))
                   .ToArray();
    }

    private Task OnMessageReceivedAsync(OnMessageReceivedArgs e, in CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.ChatMessage.Channel);
        Viewer viewer = Viewer.FromString(e.ChatMessage.Username);

        string message = e.ChatMessage.Message;

        return this.HandleChatMessageAsync(streamer: streamer, viewer: viewer, message: message, cancellationToken: cancellationToken);
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
}