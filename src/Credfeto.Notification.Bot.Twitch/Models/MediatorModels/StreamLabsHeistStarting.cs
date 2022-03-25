using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models.MediatorModels;

public sealed class StreamLabsHeistStarting : INotification
{
    public StreamLabsHeistStarting(string channel)
    {
        this.Channel = channel;
    }

    public string Channel { get; }
}