using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Interfaces;

public interface ITwitchChannelState
{
    Streamer Streamer { get; }

    bool IsOnline { get; }

    bool Chatted { get; set; }

    ValueTask OnlineAsync(
        string gameName,
        DateTimeOffset startDate,
        CancellationToken cancellationToken
    );

    void Offline();
}
