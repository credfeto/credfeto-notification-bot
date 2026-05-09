using System;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.BackgroundServices.LoggingExgtensions;

internal static partial class RestoreTwitchChatConnectionWorkerLoggingExtensions
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "Failed to update twitch chat connection: {message}"
    )]
    public static partial void FailedToUpdateTwitchChatConnection(
        this ILogger<RestoreTwitchChatConnectionWorker> logger,
        string message,
        Exception exception
    );
}
