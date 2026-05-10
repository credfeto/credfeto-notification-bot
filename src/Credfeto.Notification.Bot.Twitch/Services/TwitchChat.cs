using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
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
using TwitchLib.Client.Events;
using TwitchLib.Client.Interfaces;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using OnConnectedEventArgs = TwitchLib.Client.Events.OnConnectedEventArgs;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchChat : ITwitchChat, IDisposable
{
    private readonly TwitchClient _client;

    private readonly ConcurrentDictionary<Streamer, bool> _joinedStreamers;
    private readonly ConcurrentDictionary<Streamer, string> _lastMessage;
    private readonly SemaphoreSlim _lastMessageLock;
    private readonly ILogger<TwitchChat> _logger;
    private readonly IMediator _mediator;

    private readonly TwitchBotOptions _options;

    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    private readonly ITwitchStreamStateManager _twitchStreamStateManager;

    private readonly CancellationTokenSource _cts;
    private bool _connected;

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
            twitchStreamStateManager
            ?? throw new ArgumentNullException(nameof(twitchStreamStateManager));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        this._client =
            twitchClient as TwitchClient ?? throw new ArgumentNullException(nameof(twitchClient));

        this._cts = new();
        this._lastMessageLock = new(initialCount: 1, maxCount: 1);
        this._joinedStreamers = new();
        this._lastMessage = new();

        TwitchAuthenticationChat chatApi = this._options.Authentication.Chat;
        ConnectionCredentials credentials = new(
            twitchUsername: chatApi.UserName,
            twitchOAuth: chatApi.OAuthToken
        );

        this._client.Initialize(
            credentials: credentials,
            [this._options.Authentication.Chat.UserName]
        );

        this._joinedStreamers.TryAdd(
            Streamer.FromString(this._options.Authentication.Chat.UserName),
            value: true
        );

        // HEALTH
        this.SubscribeToChatConnection();
        this.SubscribeToChatDisconnection();

        // CHAT
        this.SubscribeToIncomingChatMessages();

        // MESSAGES BEING SENT
        this.SubscribeToOutgoingChatMessages();

        _ = this._client
            .ConnectAsync()
            .ContinueWith(
                t =>
                {
                    if (t.Exception is not null)
                    {
                        this._logger.FailedToConnect(t.Exception.GetBaseException());
                    }
                },
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.Default
            );
    }

    public void Dispose()
    {
        this._cts.Cancel();
        this._lastMessageLock.Dispose();
        this._cts.Dispose();
    }

    public void JoinChat(Streamer streamer)
    {
        this._joinedStreamers.TryAdd(key: streamer, value: true);

        _ = this._client.JoinChannelAsync(streamer.Value);
    }

    public void LeaveChat(Streamer streamer)
    {
        if (
            StringComparer.OrdinalIgnoreCase.Equals(
                x: this._options.Authentication.Chat.UserName,
                y: streamer.Value
            )
        )
        {
            // never leave own channel.
            return;
        }

        this._joinedStreamers.TryRemove(key: streamer, value: out _);

        _ = this._client.LeaveChannelAsync(streamer.Value);
    }

    public Task UpdateAsync()
    {
        if (!this._connected)
        {
            this._logger.LogDebug("Chat Reconnecting...");

            this._connected = true;
        }

        this.ReconnectToJoinedChats();

        return Task.CompletedTask;
    }

    private static IObservable<T> CreateFromAsyncEvent<T>(
        Action<AsyncEventHandler<T>> add,
        Action<AsyncEventHandler<T>> remove
    )
    {
        return Observable.Create<T>(
            async (observer, cancellationToken) =>
            {
                Task HandlerAsync(object? sender, T e)
                {
                    observer.OnNext(e);
                    return Task.CompletedTask;
                }

                add(HandlerAsync);

                TaskCompletionSource tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
                await using CancellationTokenRegistration registration = cancellationToken.Register(
                    () => tcs.TrySetResult()
                );
                await tcs.Task.ConfigureAwait(false);

                remove(HandlerAsync);
            }
        );
    }

    private void SubscribeToOutgoingChatMessages()
    {
        this._twitchChatMessageChannel.ReadAllAsync(this._cts.Token)
            .ToObservable()
            .Delay(d => Observable.Timer(this.CalculateWithJitter(d)))
            .Where(this.IsConnectedToChat)
            .Select(message =>
                Observable.FromAsync(cancellationToken =>
                    this.PublishChatMessageAsync(
                        twitchChatMessage: message,
                        cancellationToken: cancellationToken
                    )
                )
            )
            .Concat()
            .Subscribe(this._cts.Token);
    }

    private void SubscribeToIncomingChatMessages()
    {
        CreateFromAsyncEvent<OnMessageReceivedArgs>(
                h => this._client.OnMessageReceived += h,
                h => this._client.OnMessageReceived -= h
            )
            .Select(e =>
                Observable.FromAsync(cancellationToken =>
                    this.OnMessageReceivedAsync(e: e, cancellationToken: cancellationToken)
                )
            )
            .Concat()
            .Subscribe(this._cts.Token);
    }

    private void SubscribeToChatDisconnection()
    {
        CreateFromAsyncEvent<OnDisconnectedArgs>(
                h => this._client.OnDisconnected += h,
                h => this._client.OnDisconnected -= h
            )
            .Subscribe(onNext: this.OnDisconnected, this._cts.Token);
    }

    private void SubscribeToChatConnection()
    {
        CreateFromAsyncEvent<OnConnectedEventArgs>(
                h => this._client.OnConnected += h,
                h => this._client.OnConnected -= h
            )
            .Select(e => Observable.FromAsync(_ => this.OnConnectedAsync(e)))
            .Concat()
            .Subscribe(this._cts.Token);
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

    private async Task PublishChatMessageAsync(
        TwitchChatMessage twitchChatMessage,
        CancellationToken cancellationToken
    )
    {
        await this._lastMessageLock.WaitAsync(cancellationToken);

        try
        {
            if (
                this._lastMessage.TryGetValue(
                    key: twitchChatMessage.Streamer,
                    out string? lastMessage
                )
                && StringComparer.OrdinalIgnoreCase.Equals(
                    x: lastMessage,
                    y: twitchChatMessage.Message
                )
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

            this._lastMessage.TryAdd(
                key: twitchChatMessage.Streamer,
                value: twitchChatMessage.Message
            );
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

    private async Task OnConnectedAsync(OnConnectedEventArgs e)
    {
        this._logger.ChatConnected(username: e.BotUsername);
        this._connected = true;
        await this._client.JoinChannelAsync(this._options.Authentication.Chat.UserName);
    }

    private void OnDisconnected(OnDisconnectedArgs e)
    {
        this._logger.ChatDisconnected();
        this._connected = false;
    }

    private void ReconnectToJoinedChats()
    {
        // Join all the previously joined channels if not already connected
        IReadOnlyList<Streamer> streamers = this.GetPreviouslyJoinedStreamers();

        foreach (Streamer streamer in streamers)
        {
            this._logger.ChatReconnecting(streamer);
            this.JoinChat(streamer);
        }
    }

    private IReadOnlyList<Streamer> GetPreviouslyJoinedStreamers()
    {
        return [.. this._joinedStreamers.Keys.Where(streamer => !this.IsConnectedToChat(streamer))];
    }

    private async Task OnMessageReceivedAsync(
        OnMessageReceivedArgs e,
        CancellationToken cancellationToken
    )
    {
        Streamer streamer = Streamer.FromString(e.ChatMessage.Channel);
        Viewer viewer = Viewer.FromString(e.ChatMessage.Username);

        ITwitchChannelState state = this._twitchStreamStateManager.Get(streamer);

        if (
            StringComparer.OrdinalIgnoreCase.Equals(
                x: viewer.Value,
                y: this._options.Authentication.Chat.UserName
            )
        )
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
            TwitchIncomingMessage incomingMessage = new(
                Streamer: streamer,
                Chatter: viewer,
                Message: message
            );
            await this._mediator.Publish(
                notification: incomingMessage,
                cancellationToken: cancellationToken
            );
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
