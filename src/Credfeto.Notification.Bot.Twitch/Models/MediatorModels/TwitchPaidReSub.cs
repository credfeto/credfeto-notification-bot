using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models.MediatorModels;

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