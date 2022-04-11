using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchStreamRaided : INotification
{
    public TwitchStreamRaided(in Channel channel, in User raider, int viewerCount)
    {
        this.Channel = channel;
        this.Raider = raider;
        this.ViewerCount = viewerCount;
    }

    public Channel Channel { get; }

    public User Raider { get; }

    public int ViewerCount { get; }
}