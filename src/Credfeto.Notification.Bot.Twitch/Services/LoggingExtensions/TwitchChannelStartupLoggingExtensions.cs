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
}
