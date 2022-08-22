using System.Diagnostics;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

[DebuggerDisplay("{Streamer}: {Chatter} - {Message}")]
public sealed record TwitchIncomingMessage(Streamer Streamer, Viewer Chatter, string Message) : INotification;