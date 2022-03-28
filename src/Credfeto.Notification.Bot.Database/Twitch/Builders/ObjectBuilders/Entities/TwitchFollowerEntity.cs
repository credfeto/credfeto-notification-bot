using System.Diagnostics;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;

[DebuggerDisplay("{Channel}:{Follower} Followed {Follow_Count} times Fresh: {Freshly_Reached")]
public sealed record TwitchFollowerEntity
{
    public string? Channel { get; init; }

    public string? Follower { get; init; }

    // ReSharper disable once InconsistentNaming
    public int Follow_Count { get; init; }

    // ReSharper disable once InconsistentNaming
    public bool Freshly_Reached { get; init; }
}