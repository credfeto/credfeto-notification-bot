using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models.MediatorModels;

public sealed class TwitchStreamNewChatter : INotification
{
    public TwitchStreamNewChatter(string channel, string user, bool isRegular)
    {
        this.Channel = channel;
        this.User = user;
        this.IsRegular = isRegular;
    }

    public string Channel { get; }

    public string User { get; }

    public bool IsRegular { get; }
}