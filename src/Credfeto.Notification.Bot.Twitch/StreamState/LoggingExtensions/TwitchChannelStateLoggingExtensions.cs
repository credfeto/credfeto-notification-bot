using Credfeto.Notification.Bot.Twitch.DataTypes;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.StreamState.LoggingExtensions;

internal static partial class TwitchChannelStateLoggingExtensions
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "{streamer}: Going online...")]
    public static partial void ChannelGoingOnline(this ILogger logger, Streamer streamer);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "{streamer}: Going offline...")]
    public static partial void ChannelGoingOffline(this ILogger logger, Streamer streamer);
}