using System;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchChannelStartupEvent : INotification
{
    public TwitchChannelStartupEvent(TwitchUser info)
    {
        this.Info = info ?? throw new ArgumentNullException(nameof(info));
    }

    public TwitchUser Info { get; }
}