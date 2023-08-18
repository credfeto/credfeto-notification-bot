using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Data.Interfaces;

public interface ITwitchStreamerDataManager
{
    ValueTask AddStreamerAsync(Streamer streamerName, string streamerId, DateTimeOffset startedStreaming, CancellationToken cancellationToken);

    ValueTask<TwitchUser?> GetByUserNameAsync(Streamer userName, CancellationToken cancellationToken);

    ValueTask<TwitchUser?> GetByUserNameAsync(in Viewer userName, in CancellationToken cancellationToken);
}