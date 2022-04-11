using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchGiftSubSingle : INotification
{
    public TwitchGiftSubSingle(in Streamer streamer, in Viewer user)
    {
        this.Streamer = streamer;
        this.User = user;
    }

    public Streamer Streamer { get; }

    public Viewer User { get; }
}