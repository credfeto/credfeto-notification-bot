using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class MarblesStarting : INotification
{
    public MarblesStarting(in Streamer streamer)
    {
        this.Streamer = streamer;
    }

    public Streamer Streamer { get; }
}