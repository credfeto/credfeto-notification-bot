using System;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class MarblesStarting : INotification
{
    public MarblesStarting(in Streamer streamer, string message)
    {
        this.Streamer = streamer;
        this.Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    public Streamer Streamer { get; }

    public string Message { get; }
}