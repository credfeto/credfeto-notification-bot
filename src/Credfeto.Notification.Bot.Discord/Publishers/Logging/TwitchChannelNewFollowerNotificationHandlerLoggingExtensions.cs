using Credfeto.Notification.Bot.Twitch.DataTypes;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Discord.Publishers.Logging;

internal static partial class TwitchChannelNewFollowerNotificationHandlerLoggingExtensions
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "{streamer} Queue new follower message to Discord")]
    public static partial void LogQueueNewFollower(this ILogger<TwitchChannelNewFollowerNotificationHandler> logger, in Streamer streamer);
}