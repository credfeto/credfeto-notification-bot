using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models.MediatorModels;

public sealed class TwitchStreamRaided : INotification
{
    public TwitchStreamRaided(string channel, string raider, int viewerCount)
    {
        this.Channel = channel;
        this.Raider = raider;
        this.ViewerCount = viewerCount;
    }

    public string Channel { get; }

    public string Raider { get; }

    public int ViewerCount { get; }
}