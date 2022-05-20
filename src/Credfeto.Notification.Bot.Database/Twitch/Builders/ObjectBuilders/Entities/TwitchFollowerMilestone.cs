using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;

[DebuggerDisplay("{Channel}: Followers: {Followers} New {Freshly_Reached}")]
public sealed class TwitchFollowerMilestoneEntity
{
    public string? Channel { get; init; }

    public int Followers { get; init; }

    [SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming", Justification = "TODO: Review")]
    public bool Freshly_Reached { get; init; }
}