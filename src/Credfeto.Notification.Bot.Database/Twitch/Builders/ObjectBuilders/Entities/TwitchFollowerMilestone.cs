using System.Diagnostics;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;

[DebuggerDisplay("{Channel}: Followers: {Followers} New {Freshly_Reached}")]
public sealed class TwitchFollowerMilestoneEntity
{
    // ReSharper disable once InconsistentNaming
    public string? Channel { get; init; }

    // ReSharper disable once InconsistentNaming
    public int Followers { get; init; }

    // ReSharper disable once InconsistentNaming
    public bool Freshly_Reached { get; init; }
}