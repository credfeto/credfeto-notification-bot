namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchFriendChannel
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public string Channel { get; init; } = default!;

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public string? Message { get; init; }
}