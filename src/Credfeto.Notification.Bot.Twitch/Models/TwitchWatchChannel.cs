using System;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchWatchChannel : INotification
{
    public TwitchWatchChannel(TwitchUser info)
    {
        this.Info = info ?? throw new ArgumentNullException(nameof(info));
    }

    public TwitchUser Info { get; }
}