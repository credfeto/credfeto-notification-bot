using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class StreamLabsMarblesStarting : INotification
{
    public StreamLabsMarblesStarting(in Streamer streamer)
    {
        this.Streamer = streamer;
    }

    public Streamer Streamer { get; }
}