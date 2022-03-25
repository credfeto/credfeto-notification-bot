namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchBitsGift : INotification
{
    public TwitchBitsGift(string channel, string user, int bits)
    {
        this.Channel = channel;
        this.User = user;
        this.Bits = bits;
    }

    public string Channel { get; }

    public string User { get; }

    public int Bits { get; }
}