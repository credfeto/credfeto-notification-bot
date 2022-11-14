using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Discord.BackgroundServices.Logging;

internal static partial class DiscordTestServicesLoggingExtensions
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "Starting {serviceName}")]
    public static partial void LogStarting(this ILogger<DiscordTestServices> logger, string serviceName);
}