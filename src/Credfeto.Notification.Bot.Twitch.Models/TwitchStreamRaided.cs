using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchStreamRaided : INotification
{
    public TwitchStreamRaided(in Streamer streamer, in Viewer raider, int viewerCount)
    {
        this.Streamer = streamer;
        this.Raider = raider;
        this.ViewerCount = viewerCount;
    }

    public Streamer Streamer { get; }

    public Viewer Raider { get; }

    public int ViewerCount { get; }
}