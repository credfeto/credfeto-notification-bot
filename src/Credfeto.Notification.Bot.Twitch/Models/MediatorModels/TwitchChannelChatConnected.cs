using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models.MediatorModels;

public sealed class TwitchChannelChatConnected : IMediator, INotification
{
    public TwitchChannelChatConnected(string channel)
    {
        this.Channel = channel;
    }

    public string Channel { get; }
}