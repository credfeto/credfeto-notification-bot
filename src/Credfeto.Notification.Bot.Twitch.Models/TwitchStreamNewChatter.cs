using Credfeto.Notification.Bot.Twitch.DataTypes;
using Mediator;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchStreamNewChatter : INotification
{
    public TwitchStreamNewChatter(in Streamer streamer, in Viewer user, bool isRegular)
    {
        this.Streamer = streamer;
        this.User = user;
        this.IsRegular = isRegular;
    }

    public Streamer Streamer { get; }

    public Viewer User { get; }

    public bool IsRegular { get; }
}