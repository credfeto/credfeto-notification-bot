using Credfeto.Notification.Bot.Twitch.DataTypes;
using Mediator;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchBitsGift : INotification
{
    public TwitchBitsGift(in Streamer streamer, in Viewer user, int bits)
    {
        this.Streamer = streamer;
        this.User = user;
        this.Bits = bits;
    }

    public Streamer Streamer { get; }

    public Viewer User { get; }

    public int Bits { get; }
}