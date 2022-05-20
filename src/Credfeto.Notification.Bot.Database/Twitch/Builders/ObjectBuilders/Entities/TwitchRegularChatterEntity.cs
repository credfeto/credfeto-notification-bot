using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;

[DebuggerDisplay("{Chat_User}: {Regular}")]
public sealed record TwitchRegularChatterEntity
{
    [SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming", Justification = "TODO: Review")]
    public string? Chat_User { get; init; }

    public bool Regular { get; init; }
}