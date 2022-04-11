using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchNewPrimeSub : INotification
{
    public TwitchNewPrimeSub(in Channel channel, in User user)
    {
        this.Channel = channel;
        this.User = user;
    }

    public Channel Channel { get; }

    public User User { get; }
}