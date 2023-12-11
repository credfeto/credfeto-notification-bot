using System.Diagnostics;
using Credfeto.Database.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.DataTypes.Mappers;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;

[DebuggerDisplay("{Streamer}: Followers: {Followers} New {FreshlyReached}")]
public sealed record TwitchFollowerMilestone(
    [SqlFieldMap<StreamerMapper, Streamer>]
    Streamer Streamer,
    int Followers,
    bool FreshlyReached);