using System;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models.MediatorModels;

public sealed class TwitchStreamOffline : INotification
{
    public TwitchStreamOffline(string channel, string title, string gameName, in DateTime startedAt)
    {
        this.Channel = channel;
        this.Title = title;
        this.GameName = gameName;
        this.StartedAt = startedAt;
    }

    public string Channel { get; }

    public string Title { get; }

    public string GameName { get; }

    public DateTime StartedAt { get; }
}