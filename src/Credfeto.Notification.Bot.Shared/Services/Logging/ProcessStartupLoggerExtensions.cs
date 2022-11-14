using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Shared.Services.Logging;

internal static partial class ProcessStartupLoggerExtensions
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Starting...")]
    public static partial void LogStarting(this ILogger<ProcessStartup> logger);
}