using System;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Services.LoggingExtensions;

internal static partial class TwitchStreamStatusLoggingExtensions
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Warning,
        Message = "{streamer}: Started streaming \"{title}\" ({gameName}) at {startedAt}"
    )]
    public static partial void StreamStarted(
        this ILogger<TwitchStreamStatus> logger,
        Streamer streamer,
        string title,
        string gameName,
        DateTimeOffset startedAt
    );

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Error,
        Message = "{streamer}: Failed to notify stream started: {message}"
    )]
    public static partial void FailedToNotifyStreamStarted(
        this ILogger<TwitchStreamStatus> logger,
        Streamer streamer,
        string message,
        Exception exception
    );

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Error,
        Message = "{streamer}: Not Found: {message}"
    )]
    public static partial void StreamertNotFound(
        this ILogger<TwitchStreamStatus> logger,
        Streamer streamer,
        string message,
        Exception exception
    );

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Error,
        Message = "{streamer}: Failed to notify stream stopped: {message}"
    )]
    public static partial void FailedToNotifyStreamStopped(
        this ILogger<TwitchStreamStatus> logger,
        Streamer streamer,
        string message,
        Exception exception
    );
}
