using Credfeto.Notification.Bot.Twitch.DataTypes;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers.LoggingExtensions;

internal static partial class TwitchWatchChannelLiveStreamNotificationHandlerLoggingExtensions
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "{streamer}: Enabling for live status checks")]
    public static partial void EnablingForLiveStatusChecks(this ILogger<TwitchWatchChannelLiveStreamNotificationHandler> logger, Streamer streamer);
}