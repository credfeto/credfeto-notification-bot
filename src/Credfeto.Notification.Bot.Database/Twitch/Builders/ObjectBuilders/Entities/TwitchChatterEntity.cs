using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;

[DebuggerDisplay("{Channel}: {Chat_User} {First_Message_Date} Stream Started: {Start_Date}")]
public sealed record TwitchChatterEntity
{
    public string? Channel { get; init; }

    [SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming", Justification = "TODO: Review")]
    public DateTime Start_Date { get; init; }

    [SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming", Justification = "TODO: Review")]
    public string? Chat_User { get; init; }

    [SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming", Justification = "TODO: Review")]
    public DateTime First_Message_Date { get; init; }
}