using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;

[DebuggerDisplay("{Channel}: Thanks: {Thanks}")]
public sealed record StreamSettingsEntity
{
    public string? Channel { get; init; }

    public bool Thanks { get; init; }

    [SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming", Justification = "Matches name in database")]
    public bool Announce_Milestones { get; init; }

    [SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming", Justification = "Matches name in database")]
    public bool Chat_Welcomes { get; init; }

    [SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming", Justification = "Matches name in database")]
    public bool Raid_Welcomes { get; init; }

    [SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming", Justification = "Matches name in database")]
    public bool Shout_Outs { get; init; }
}