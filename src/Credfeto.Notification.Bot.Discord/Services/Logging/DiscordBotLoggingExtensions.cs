using System;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Discord.Services.Logging;

internal static partial class DiscordBotLoggingExtensions
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Debug, Message = "{streamer}: Queuing message for Discord")]
    public static partial void LogQueueMessage(this ILogger<DiscordBot> logger, in string streamer);

    [LoggerMessage(EventId = 2, Level = LogLevel.Critical, Message = "{streamer}: Error queuing message for Discord: {message}")]
    public static partial void LogQueueMessageError(this ILogger<DiscordBot> logger, in string streamer, string message, Exception exception);
}