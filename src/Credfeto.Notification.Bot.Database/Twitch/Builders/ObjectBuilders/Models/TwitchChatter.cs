using System;
using System.Diagnostics;
using Credfeto.Database.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.DataTypes.Mappers;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;

[DebuggerDisplay("{Streamer}: {Viewer} {FirstMessage} Stream Started: {StreamStartDate}")]
public sealed record TwitchChatter(
    [SqlFieldMap<StreamerMapper, Streamer>]
    Streamer Streamer,
    [SqlFieldMap<DateTimeOffsetMapper, DateTimeOffset>]
    DateTimeOffset StreamStartDate,
    [SqlFieldMap<ViewerMapper, Viewer>] Viewer Viewer,
    [SqlFieldMap<DateTimeOffsetMapper, DateTimeOffset>]
    DateTimeOffset FirstMessage);