using System;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Mediator;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchStreamOnline : INotification
{
    public TwitchStreamOnline(
        in Streamer streamer,
        string title,
        string gameName,
        in DateTime startedAt
    )
    {
        this.Streamer = streamer;
        this.Title = title;
        this.GameName = gameName;
        this.StartedAt = startedAt;
    }

    public Streamer Streamer { get; }

    public string Title { get; }

    public DateTime StartedAt { get; }

    public string GameName { get; }
}
