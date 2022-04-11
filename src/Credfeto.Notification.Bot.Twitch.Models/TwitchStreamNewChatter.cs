using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchStreamNewChatter : INotification
{
    public TwitchStreamNewChatter(in Channel channel, in User user, bool isRegular)
    {
        this.Channel = channel;
        this.User = user;
        this.IsRegular = isRegular;
    }

    public Channel Channel { get; }

    public User User { get; }

    public bool IsRegular { get; }
}