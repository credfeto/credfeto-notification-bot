using System;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Mediator;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchWatchChannel : INotification
{
    public TwitchWatchChannel(TwitchUser info)
    {
        this.Info = info ?? throw new ArgumentNullException(nameof(info));
    }

    public TwitchUser Info { get; }
}