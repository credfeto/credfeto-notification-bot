namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchPaidReSub : INotification
{
    public TwitchPaidReSub(string channel, string user)
    {
        this.Channel = channel;
        this.User = user;
    }

    public string Channel { get; }

    public string User { get; }
}