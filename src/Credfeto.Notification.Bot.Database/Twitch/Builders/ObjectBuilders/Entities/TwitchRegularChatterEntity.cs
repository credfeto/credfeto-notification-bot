using System.Diagnostics;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;

[DebuggerDisplay("{Chat_User}: {Regular}")]
public sealed record TwitchRegularChatterEntity
{
    // ReSharper disable once InconsistentNaming
    public string? Chat_User { get; init; }

    public bool Regular { get; init; }
}