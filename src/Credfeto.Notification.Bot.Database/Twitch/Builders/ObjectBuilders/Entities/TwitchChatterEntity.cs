using System;
using System.Diagnostics;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;

[DebuggerDisplay("{Channel}: {Chat_User} {First_Message_Date} Stream Started: {Start_Date}")]
public sealed class TwitchChatterEntity
{
    public string? Channel { get; init; }

    // ReSharper disable once InconsistentNaming
    public DateTime Start_Date { get; init; }

    // ReSharper disable once InconsistentNaming
    public string? Chat_User { get; init; }

    // ReSharper disable once InconsistentNaming
    public DateTime? First_Message_Date { get; init; }
}