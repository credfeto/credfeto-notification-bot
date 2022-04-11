using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class StreamLabsHeistStarting : INotification
{
    public StreamLabsHeistStarting(in Streamer streamer)
    {
        this.Streamer = streamer;
    }

    public Streamer Streamer { get; }
}