using Credfeto.Notification.Bot.Twitch.DataTypes;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Services.LoggingExtensions;

internal static partial class TwitchMessageTriggerDebounceFilterLoggingExtensions
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "{streamer}: {message}  Can Send: {canSend}")]
    public static partial void CanSend(
        this ILogger<TwitchMessageTriggerDebounceFilter> logger,
        Streamer streamer,
        string message,
        bool canSend
    );
}
