using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models.MediatorModels;

public sealed class TwitchChannelNewFollower : INotification
{
    public TwitchChannelNewFollower(string channel, string user, bool streamOnline)
    {
        this.Channel = channel;
        this.User = user;
        this.StreamOnline = streamOnline;
    }

    public string Channel { get; }

    public string User { get; }

    public bool StreamOnline { get; }
}