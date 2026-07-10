using System;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Startup;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Services.LoggingExtensions;

internal static partial class TwitchChannelStartupLoggingExtensions
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "{streamer}: Looking for channel")]
    public static partial void LookingForChannel(this ILogger<TwitchChannelStartup> logger, Streamer streamer);

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "{streamer}: Found channel {userId}, IsStreamer: {isStreamer}, Date Created: {dateCreated}"
    )]
    public static partial void FoundChannel(
        this ILogger<TwitchChannelStartup> logger,
        Streamer streamer,
        int userId,
        bool isStreamer,
        DateTimeOffset dateCreated
    );

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Warning,
        Message = "{streamer}: Retrying channel lookup (attempt {attempt}) after failure: {message}"
    )]
    public static partial void RetryingChannelLookup(
        this ILogger<TwitchChannelStartup> logger,
        Streamer streamer,
        int attempt,
        string message,
        Exception exception
    );

    [LoggerMessage(
        EventId = 4,
        Level = LogLevel.Error,
        Message = "{streamer}: Giving up on channel lookup after {attempts} attempts: {message}"
    )]
    public static partial void GivingUpOnChannelLookup(
        this ILogger<TwitchChannelStartup> logger,
        Streamer streamer,
        int attempts,
        string message,
        Exception exception
    );
}
