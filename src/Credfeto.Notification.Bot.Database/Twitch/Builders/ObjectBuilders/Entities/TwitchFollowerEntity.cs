using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;

[DebuggerDisplay("{Channel}:{Follower} Followed {Follow_Count} times Fresh: {Freshly_Reached")]
public sealed record TwitchFollowerEntity
{
    public string? Channel { get; init; }

    public string? Follower { get; init; }

    [SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming", Justification = "TODO: Review")]
    public int Follow_Count { get; init; }

    [SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming", Justification = "TODO: Review")]
    public bool Freshly_Reached { get; init; }
}