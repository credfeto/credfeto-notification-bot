using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchGiftSubMultiple : INotification
{
    public TwitchGiftSubMultiple(in Channel channel, in User user, int count)
    {
        this.Channel = channel;
        this.User = user;
        this.Count = count;
    }

    public Channel Channel { get; }

    public User User { get; }

    public int Count { get; }
}