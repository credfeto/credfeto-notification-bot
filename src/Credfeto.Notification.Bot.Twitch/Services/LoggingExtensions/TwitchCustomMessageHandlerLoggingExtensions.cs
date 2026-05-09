using Credfeto.Notification.Bot.Twitch.DataTypes;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Services.LoggingExtensions;

internal static partial class TwitchCustomMessageHandlerLoggingExtensions
{
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "{streamer}: Debouncing... {chatter}: {message}"
    )]
    public static partial void Debouncing(
        this ILogger<TwitchCustomMessageHandler> logger,
        Streamer streamer,
        Viewer chatter,
        string message
    );

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Information,
        Message = "{streamer}: Matched {chatter}: {message}"
    )]
    public static partial void Matched(
        this ILogger<TwitchCustomMessageHandler> logger,
        Streamer streamer,
        Viewer chatter,
        string message
    );

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Information,
        Message = "{streamer}: Checking match \"{chatter}\" : Pattern: \"{triggerMessage}\" : Message: \"{sendMessage}\" : Match: {isMatch}"
    )]
    public static partial void CheckingMatch(
        this ILogger<TwitchCustomMessageHandler> logger,
        Streamer streamer,
        Viewer chatter,
        string triggerMessage,
        string sendMessage,
        bool isMatch
    );

    [LoggerMessage(
        EventId = 3,
        Level = LogLevel.Information,
        Message = "{streamer}: Adding Chat command \"{chatter}\" : Pattern: \"{triggerMessage}\""
    )]
    public static partial void AddingChatCommandTrigger(
        this ILogger<TwitchCustomMessageHandler> logger,
        Streamer streamer,
        Viewer chatter,
        string triggerMessage
    );
}
