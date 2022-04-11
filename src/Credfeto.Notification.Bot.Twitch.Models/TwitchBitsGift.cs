using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchBitsGift : INotification
{
    public TwitchBitsGift(in Channel channel, in User user, int bits)
    {
        this.Channel = channel;
        this.User = user;
        this.Bits = bits;
    }

    public Channel Channel { get; }

    public User User { get; }

    public int Bits { get; }
}