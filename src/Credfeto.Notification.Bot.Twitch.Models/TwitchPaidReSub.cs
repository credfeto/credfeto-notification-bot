using Credfeto.Notification.Bot.Twitch.DataTypes;
using Mediator;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchPaidReSub : INotification
{
    public TwitchPaidReSub(in Streamer streamer, in Viewer user)
    {
        this.Streamer = streamer;
        this.User = user;
    }

    public Streamer Streamer { get; }

    public Viewer User { get; }
}