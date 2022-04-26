using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Actions;

public interface IHoster
{
    Task StreamOnlineAsync(Streamer streamer, DateTime streamStartTime, CancellationToken cancellationToken);

    Task StreamOfflineAsync(Streamer streamer, CancellationToken cancellationToken);
}