using System.Diagnostics;
using Credfeto.Database.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.DataTypes.Mappers;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;

[DebuggerDisplay("{Viewer}: {IsRegular}")]
public sealed record TwitchRegularChatter([SqlFieldMap<ViewerMapper, Viewer>] Viewer Viewer, bool IsRegular);