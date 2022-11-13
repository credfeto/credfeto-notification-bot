using Credfeto.Notification.Bot.Twitch.DataTypes;
using Mediator;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchGiftSubMultiple : INotification
{
    public TwitchGiftSubMultiple(in Streamer streamer, in Viewer user, int count)
    {
        this.Streamer = streamer;
        this.User = user;
        this.Count = count;
    }

    public Streamer Streamer { get; }

    public Viewer User { get; }

    public int Count { get; }
}