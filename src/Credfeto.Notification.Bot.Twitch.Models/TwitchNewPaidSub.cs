using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchNewPaidSub : INotification
{
    public TwitchNewPaidSub(in Channel channel, in User user)
    {
        this.Channel = channel;
        this.User = user;
    }

    public Channel Channel { get; }

    public User User { get; }
}