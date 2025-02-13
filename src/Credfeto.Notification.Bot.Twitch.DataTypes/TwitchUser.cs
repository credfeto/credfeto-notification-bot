using System;
using System.Diagnostics;
using Credfeto.Database.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes.Mappers;

namespace Credfeto.Notification.Bot.Twitch.DataTypes;

[DebuggerDisplay("{UserName}: Id: {Id} Streamer: {IsStreamer} Created: {DateCreated}")]
public sealed record TwitchUser(
    int Id,
    [SqlFieldMap<ViewerMapper, Viewer>] Viewer UserName,
    bool IsStreamer,
    [SqlFieldMap<DateTimeOffsetMapper, DateTimeOffset>] in DateTimeOffset DateCreated
);
