namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchChannelChatConnected : INotification
{
    public TwitchChannelChatConnected(string channel)
    {
        this.Channel = channel;
    }

    public string Channel { get; }
}