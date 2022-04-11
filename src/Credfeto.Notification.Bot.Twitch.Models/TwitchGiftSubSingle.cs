using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchGiftSubSingle : INotification
{
    public TwitchGiftSubSingle(in Channel channel, in User user)
    {
        this.Channel = channel;
        this.User = user;
    }

    public Channel Channel { get; }

    public User User { get; }
}