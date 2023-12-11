using System.Diagnostics;
using Credfeto.Database.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.DataTypes.Mappers;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;

[DebuggerDisplay("{Streamer}:{Viewer} Followed {FollowCount} times Fresh: {FreshlyReached")]
public sealed record TwitchFollower(
    [SqlFieldMap<StreamerMapper, Streamer>]
    Streamer Streamer,
    [SqlFieldMap<ViewerMapper, Viewer>] Viewer Viewer,
    int FollowCount,
    bool FreshlyReached);