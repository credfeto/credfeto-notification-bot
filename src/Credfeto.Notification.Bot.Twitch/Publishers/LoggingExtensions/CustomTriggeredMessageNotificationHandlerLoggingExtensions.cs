using System;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers.LoggingExtensions;

internal static partial class CustomTriggeredMessageNotificationHandlerLoggingExtensions
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "{streamer}: Failed to send custom message: {message}")]
    public static partial void FailedToSendCustomMessage(this ILogger<CustomTriggeredMessageNotificationHandler> logger, Streamer streamer, string message, Exception exception);
}