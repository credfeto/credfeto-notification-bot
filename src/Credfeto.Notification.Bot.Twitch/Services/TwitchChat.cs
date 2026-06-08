using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Date.Interfaces;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Services.LoggingExtensions;
using Credfeto.Notification.Bot.Twitch.StreamState;
using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NonBlocking;
using TwitchLib.Client;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using OnConnectedEventArgs = TwitchLib.Client.Events.OnConnectedEventArgs;
using OnDisconnectedArgs = TwitchLib.Client.Events.OnDisconnectedArgs;
using OnMessageReceivedArgs = TwitchLib.Client.Events.OnMessageReceivedArgs;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchChat : ITwitchChat, IDisposable
{
    private readonly IDisposable _chatConnected;
    private readonly IDisposable _chatDisconnected;
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

    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    private readonly ITwitchStreamStateManager _twitchStreamStateManager;

    private bool _connected;

    [SuppressMessage(
        category: "Microsoft.VisualStudio.Threading",
        checkId: "VSTHRD002: Avoid problematic synchronous waits",
        Justification = "Constructor cannot be async"
    )]
    public TwitchChat(
        IOptions<TwitchBotOptions> options,
        IMessageChannel<TwitchChatMessage> twitchChatMessageChannel,
        IMediator mediator,
        ITwitchClient twitchClient,
        ITwitchStreamStateManager twitchStreamStateManager,
        ILogger<TwitchChat> logger
    )
    {
        this._twitchChatMessageChannel = twitchChatMessageChannel;
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this._twitchStreamStateManager =
            twitchStreamStateManager ?? throw new ArgumentNullException(nameof(twitchStreamStateManager));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        this._client = twitchClient as TwitchClient ?? throw new ArgumentNullException(nameof(twitchClient));

        this._lastMessageLock = new(initialCount: 1, maxCount: 1);
        this._joinedStreamers = new();
        this._lastMessage = new();

        TwitchAuthenticationChat chatApi = this._options.Authentication.Chat;
        ConnectionCredentials credentials = new(twitchUsername: chatApi.UserName, twitchOAuth: chatApi.OAuthToken);

        this._client.Initialize(credentials: credentials, [this._options.Authentication.Chat.UserName]);

        this._joinedStreamers.TryAdd(Streamer.FromString(this._options.Authentication.Chat.UserName), value: true);

        // HEALTH
        this._chatConnected = this.SubscribeToChatConnection();
        this._chatDisconnected = this.SubscribeToChatDisconnection();
        this._chatReconnected = this.SubscribeToChatReconnections();

        // CHAT
        this._chatMessageReceived = this.SubscribeToIncomingChatMessages();

        // MESSAGES BEING SENT
        this._sentChatMessages = this.SubscribeToOutgoingChatMessages();

        this._client.ConnectAsync().GetAwaiter().GetResult();
        this._connected = true;

        this._client.JoinChannelAsync(this._options.Authentication.Chat.UserName).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        this._chatConnected.Dispose();
        this._chatDisconnected.Dispose();
        this._chatMessageReceived.Dispose();
        this._chatReconnected.Dispose();
        this._lastMessageLock.Dispose();
        this._sentChatMessages.Dispose();
    }

    public Task JoinChatAsync(Streamer streamer)
    {
        this._joinedStreamers.TryAdd(key: streamer, value: true);

        return this._client.JoinChannelAsync(streamer.Value);
    }

    public Task LeaveChatAsync(Streamer streamer)
    {
        if (StringComparer.OrdinalIgnoreCase.Equals(x: this._options.Authentication.Chat.UserName, y: streamer.Value))
        {
            // never leave own channel.
            return Task.CompletedTask;
        }

        this._joinedStreamers.TryRemove(key: streamer, value: out _);

        return this._client.LeaveChannelAsync(streamer.Value);
    }

    public Task UpdateAsync()
    {
        if (!this._connected)
        {
            this._logger.LogDebug("Chat Reconnecting...");

            this._connected = true;
        }

        return this.ReconnectToJoinedChatsAsync();
    }

    [SuppressMessage(
        category: "Meziantou.Analyzer",
        checkId: "MA0032: Use CancellationToken",
        Justification = "Using IDisposable instead"
    )]
    private IDisposable SubscribeToOutgoingChatMessages()
    {
        return this
            ._twitchChatMessageChannel.ReadAllAsync(CancellationToken.None)
            .ToObservable()
            .Delay(d => Observable.Timer(this.CalculateWithJitter(d)))
            .Where(this.IsConnectedToChat)
            .Select(message =>
                Observable.FromAsync(cancellationToken =>
                    this.PublishChatMessageAsync(twitchChatMessage: message, cancellationToken: cancellationToken)
                )
            )
            .Concat()
            .Subscribe();
    }

    [SuppressMessage(
        category: "Meziantou.Analyzer",
        checkId: "MA0032: Use CancellationToken",
        Justification = "Using IDisposable instead"
    )]
    private IDisposable SubscribeToIncomingChatMessages()
    {
        return Observable
            .Create<OnMessageReceivedArgs>(observer =>
            {
                Task HandlerAsync(object? _, OnMessageReceivedArgs e)
                {
                    observer.OnNext(e);

                    return Task.CompletedTask;
                }

                AsyncEventHandler<OnMessageReceivedArgs> h = HandlerAsync;
                this._client.OnMessageReceived += h;

                return () => this._client.OnMessageReceived -= h;
            })
            .Select(e =>
                Observable.FromAsync(cancellationToken =>
                    this.OnMessageReceivedAsync(e: e, cancellationToken: cancellationToken)
                )
            )
            .Concat()
            .Subscribe();
    }

    [SuppressMessage(
        category: "Meziantou.Analyzer",
        checkId: "MA0032: Use CancellationToken",
        Justification = "Using IDisposable instead"
    )]
    private IDisposable SubscribeToChatReconnections()
    {
        return Observable
            .Create<OnConnectedEventArgs>(observer =>
            {
                Task HandlerAsync(object? _, OnConnectedEventArgs e)
                {
                    observer.OnNext(e);

                    return Task.CompletedTask;
                }

                AsyncEventHandler<OnConnectedEventArgs> h = HandlerAsync;
                this._client.OnReconnected += h;

                return () => this._client.OnReconnected -= h;
            })
            .Select(_ => Observable.FromAsync(__ => this.OnReconnectedAsync()))
            .Concat()
            .Subscribe();
    }

    [SuppressMessage(
        category: "Meziantou.Analyzer",
        checkId: "MA0032: Use CancellationToken",
        Justification = "Using IDisposable instead"
    )]
    private IDisposable SubscribeToChatDisconnection()
    {
        return Observable
            .Create<OnDisconnectedArgs>(observer =>
            {
                Task HandlerAsync(object? _, OnDisconnectedArgs e)
                {
                    observer.OnNext(e);

                    return Task.CompletedTask;
                }

                AsyncEventHandler<OnDisconnectedArgs> h = HandlerAsync;
                this._client.OnDisconnected += h;

                return () => this._client.OnDisconnected -= h;
            })
            .Subscribe(onNext: this.OnDisconnected);
    }

    [SuppressMessage(
        category: "Meziantou.Analyzer",
        checkId: "MA0032: Use CancellationToken",
        Justification = "Using IDisposable instead"
    )]
    private IDisposable SubscribeToChatConnection()
    {
        return Observable
            .Create<OnConnectedEventArgs>(observer =>
            {
                Task HandlerAsync(object? _, OnConnectedEventArgs e)
                {
                    observer.OnNext(e);

                    return Task.CompletedTask;
                }

                AsyncEventHandler<OnConnectedEventArgs> h = HandlerAsync;
                this._client.OnConnected += h;

                return () => this._client.OnConnected -= h;
            })
            .Subscribe(onNext: this.OnConnected);
    }

    private TimeSpan CalculateWithJitter(TwitchChatMessage twitchChatMessage)
    {
        MessagePriority priority = twitchChatMessage.Priority;

        double delay = Jitter.WithJitter((int)priority, GetMaxSeconds(priority));

        this._logger.DelayingMessage(
            streamer: twitchChatMessage.Streamer,
            delay: delay,
            message: twitchChatMessage.Message
        );

        return TimeSpan.FromSeconds(delay);

        static int GetMaxSeconds(MessagePriority messagePriority)
        {
            return messagePriority switch
            {
                MessagePriority.ASAP => 2 * (int)MessagePriority.ASAP,
                MessagePriority.NATURAL => 3 * (int)MessagePriority.NATURAL,
                MessagePriority.SLOW => 6 * (int)MessagePriority.SLOW,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(messagePriority),
                    actualValue: messagePriority,
                    message: "Unknown message priority"
                ),
            };
        }
    }

    private bool IsConnectedToChat(TwitchChatMessage chatMessage)
    {
        return this.IsConnectedToChat(chatMessage.Streamer);
    }

    private bool IsConnectedToChat(Streamer streamer)
    {
        return this._client.JoinedChannels.Any(joinedChannel =>
            StringComparer.OrdinalIgnoreCase.Equals(x: streamer.Value, y: joinedChannel.Channel)
        );
    }

    private async Task PublishChatMessageAsync(TwitchChatMessage twitchChatMessage, CancellationToken cancellationToken)
    {
        await this._lastMessageLock.WaitAsync(cancellationToken);

        try
        {
            if (
                this._lastMessage.TryGetValue(key: twitchChatMessage.Streamer, out string? lastMessage)
                && StringComparer.OrdinalIgnoreCase.Equals(x: lastMessage, y: twitchChatMessage.Message)
            )
            {
                if (!twitchChatMessage.IsCommand)
                {
                    return;
                }
            }

            this._lastMessage.TryRemove(key: twitchChatMessage.Streamer, value: out _);

            this._logger.SendingMessage(
                streamer: twitchChatMessage.Streamer,
                viewer: this._options.Authentication.Chat,
                message: twitchChatMessage.Message
            );

            await this._client.SendMessageAsync(
                channel: twitchChatMessage.Streamer.Value,
                message: twitchChatMessage.Message
            );

            this._lastMessage.TryAdd(key: twitchChatMessage.Streamer, value: twitchChatMessage.Message);
        }
        catch (Exception exception)
        {
            this._logger.FailedToSendMessage(
                streamer: twitchChatMessage.Streamer,
                message: exception.Message,
                exception: exception
            );
        }
        finally
        {
            this._lastMessageLock.Release();
        }
    }

    private void OnConnected(OnConnectedEventArgs e)
    {
        this._logger.ChatConnected(username: e.BotUsername);
        this._connected = true;
    }

    private void OnDisconnected(OnDisconnectedArgs _)
    {
        this._logger.ChatDisconnected();
        this._connected = false;
    }

    private Task OnReconnectedAsync()
    {
        this._logger.LogWarning("Chat Reconnected :)");
        this._connected = true;

        return this.ReconnectToJoinedChatsAsync();
    }

    private async Task ReconnectToJoinedChatsAsync()
    {
        IReadOnlyList<Streamer> streamers = this.GetPreviouslyJoinedStreamers();

        foreach (Streamer streamer in streamers)
        {
            this._logger.ChatReconnecting(streamer);
            await this.JoinChatAsync(streamer);
        }
    }

    private IReadOnlyList<Streamer> GetPreviouslyJoinedStreamers()
    {
        return [.. this._joinedStreamers.Keys.Where(streamer => !this.IsConnectedToChat(streamer))];
    }

    private async Task OnMessageReceivedAsync(OnMessageReceivedArgs e, CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.ChatMessage.Channel);
        Viewer viewer = Viewer.FromString(e.ChatMessage.Username);

        ITwitchChannelState state = this._twitchStreamStateManager.Get(streamer);

        if (StringComparer.OrdinalIgnoreCase.Equals(x: viewer.Value, y: this._options.Authentication.Chat.UserName))
        {
            state.Chatted = true;
        }

        if (state.Chatted)
        {
            string message = e.ChatMessage.Message;

            await this.HandleChatMessageAsync(
                streamer: streamer,
                viewer: viewer,
                message: message,
                cancellationToken: cancellationToken
            );
        }
    }

    private async Task HandleChatMessageAsync(
        Streamer streamer,
        Viewer viewer,
        string message,
        CancellationToken cancellationToken
    )
    {
        try
        {
            TwitchIncomingMessage incomingMessage = new(Streamer: streamer, Chatter: viewer, Message: message);
            await this._mediator.Publish(notification: incomingMessage, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.FailedToHandleChatMessage(
                streamer: streamer,
                message: exception.Message,
                exception: exception
            );
        }
    }
}
