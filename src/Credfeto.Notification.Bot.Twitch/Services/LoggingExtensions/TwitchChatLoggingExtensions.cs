using System;
using System.Diagnostics;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Services.LoggingExtensions;

internal static partial class TwitchChatLoggingExtensions
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "{streamer}: Delaying message for {delay} seconds for: {message}")]
    public static partial void DelayingMessage(this ILogger<TwitchChat> logger, Streamer streamer, double delay, string message);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "{streamer}: >>> {viewer} SEND >>> {message}")]
    public static partial void SendingMessage(this ILogger<TwitchChat> logger, Streamer streamer, Viewer viewer, string message);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "{streamer}: Failed to send message: {message}")]
    public static partial void FailedToSendMessage(this ILogger<TwitchChat> logger, Streamer streamer, string message, Exception exception);

    [Conditional("DEBUG")]
    [LoggerMessage(EventId = 4, Level = LogLevel.Error, Message = "{dateTimeOffset}: {username} - {data}")]
    public static partial void DebugLog(this ILogger<TwitchChat> logger, DateTimeOffset dateTimeOffset, string username, string data);

    [LoggerMessage(EventId = 5, Level = LogLevel.Error, Message = "{username}: Chat connected - {autoJoinChannel}")]
    public static partial void ChatConnected(this ILogger<TwitchChat> logger, string username, string autoJoinChannel);

    [LoggerMessage(EventId = 6, Level = LogLevel.Error, Message = "Chat disconnected :(")]
    public static partial void ChatDisconnected(this ILogger<TwitchChat> logger);

    [LoggerMessage(EventId = 7, Level = LogLevel.Information, Message = "{streamer}: Reconnecting to chat")]
    public static partial void ChatReconnecting(this ILogger<TwitchChat> logger, Streamer streamer);

    [LoggerMessage(EventId = 8, Level = LogLevel.Error, Message = "{streamer}: Failed to handle chat message: {message}")]
    public static partial void FailedToHandleChatMessage(this ILogger<TwitchChat> logger, Streamer streamer, string message, Exception exception);
}