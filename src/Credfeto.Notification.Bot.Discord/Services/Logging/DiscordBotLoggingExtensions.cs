using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Discord.Services.Logging;

internal static partial class DiscordBotLoggingExtensions
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "{streamer}: Queuing message for Discord")]
    public static partial void LogQueueMessage(this ILogger<DiscordBot> logger, in string streamer);
}