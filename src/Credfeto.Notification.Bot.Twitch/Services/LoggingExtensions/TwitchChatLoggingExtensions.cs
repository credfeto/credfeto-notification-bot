using System;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Services.LoggingExtensions;

internal static partial class TwitchChatLoggingExtensions
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "{streamer}: Delaying message for {delay} seconds for: {message}"
    )]
    public static partial void DelayingMessage(
        this ILogger<TwitchChat> logger,
        Streamer streamer,
        double delay,
        string message
    );

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "{streamer}: >>> {viewer} SEND >>> {message}"
    )]
    public static partial void SendingMessage(
        this ILogger<TwitchChat> logger,
        Streamer streamer,
        Viewer viewer,
        string message
    );

    public static void SendingMessage(
        this ILogger<TwitchChat> logger,
        in Streamer streamer,
        TwitchAuthenticationChat viewer,
        string message
    )
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.SendingMessage(
                streamer: streamer,
                Viewer.FromString(viewer.UserName),
                message: message
            );
        }
    }

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Error,
        Message = "{streamer}: Failed to send message: {message}"
    )]
    public static partial void FailedToSendMessage(
        this ILogger<TwitchChat> logger,
        Streamer streamer,
        string message,
        Exception exception
    );

    [LoggerMessage(EventId = 5, Level = LogLevel.Information, Message = "{username}: Chat connected")]
    public static partial void ChatConnected(this ILogger<TwitchChat> logger, string username);

    [LoggerMessage(EventId = 6, Level = LogLevel.Warning, Message = "Chat disconnected :(")]
    public static partial void ChatDisconnected(this ILogger<TwitchChat> logger);

    [LoggerMessage(EventId = 9, Level = LogLevel.Error, Message = "Failed to connect to chat")]
    public static partial void FailedToConnect(this ILogger<TwitchChat> logger, Exception exception);

    [LoggerMessage(
        EventId = 7,
        Level = LogLevel.Information,
        Message = "{streamer}: Reconnecting to chat"
    )]
    public static partial void ChatReconnecting(this ILogger<TwitchChat> logger, Streamer streamer);

    [LoggerMessage(
        EventId = 8,
        Level = LogLevel.Error,
        Message = "{streamer}: Failed to handle chat message: {message}"
    )]
    public static partial void FailedToHandleChatMessage(
        this ILogger<TwitchChat> logger,
        Streamer streamer,
        string message,
        Exception exception
    );
}
