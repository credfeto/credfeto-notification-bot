using System.Diagnostics.CodeAnalysis;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchFriendChannel
{
    [SuppressMessage(category: "ReSharper", checkId: "AutoPropertyCanBeMadeGetOnly.Global", Justification = "TODO: Review")]
    public string Channel { get; init; } = default!;

    [SuppressMessage(category: "ReSharper", checkId: "AutoPropertyCanBeMadeGetOnly.Global", Justification = "TODO: Review")]
    public string? Message { get; init; }
}