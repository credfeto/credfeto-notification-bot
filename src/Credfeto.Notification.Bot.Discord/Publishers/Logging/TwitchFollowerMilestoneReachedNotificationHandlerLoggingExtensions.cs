using Credfeto.Notification.Bot.Twitch.DataTypes;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Discord.Publishers.Logging;

internal static partial class TwitchFollowerMilestoneReachedNotificationHandlerLoggingExtensions
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "{streamer}: Woo!! New follower milestone reached {milestoneReached}")]
    public static partial void LogFolloweMilestoneReached(this ILogger<TwitchFollowerMilestoneReachedNotificationHandler> logger, in Streamer streamer, int milestoneReached);
}