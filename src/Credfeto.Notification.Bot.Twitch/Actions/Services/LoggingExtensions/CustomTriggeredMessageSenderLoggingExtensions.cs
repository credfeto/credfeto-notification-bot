using Credfeto.Notification.Bot.Twitch.DataTypes;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services.LoggingExtensions;

internal static partial class CustomTriggeredMessageSenderLoggingExtensions
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "{streamer}: Custom message triggered [Sending]: {message}"
    )]
    public static partial void CustomMessageTriggeredSending(
        this ILogger<CustomTriggeredMessageSender> logger,
        Streamer streamer,
        string message
    );

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "{streamer}: Custom message triggered [Sent]: {message}"
    )]
    public static partial void CustomMessageTriggeredSent(
        this ILogger<CustomTriggeredMessageSender> logger,
        Streamer streamer,
        string message
    );
}
