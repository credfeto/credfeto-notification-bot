using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchChannelChatConnected : INotification
{
    public TwitchChannelChatConnected(in Channel channel)
    {
        this.Channel = channel;
    }

    public Channel Channel { get; }
}