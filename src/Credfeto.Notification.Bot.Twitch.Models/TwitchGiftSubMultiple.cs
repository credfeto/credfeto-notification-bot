using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchGiftSubMultiple : INotification
{
    public TwitchGiftSubMultiple(string channel, string user, int count)
    {
        this.Channel = channel;
        this.User = user;
        this.Count = count;
    }

    public string Channel { get; }

    public string User { get; }

    public int Count { get; }
}