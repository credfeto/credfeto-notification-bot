namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchGiftSubSingle : INotification
{
    public TwitchGiftSubSingle(string channel, string user)
    {
        this.Channel = channel;
        this.User = user;
    }

    public string Channel { get; }

    public string User { get; }
}