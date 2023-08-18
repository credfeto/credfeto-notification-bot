using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Database;
using Credfeto.Notification.Bot.Database.Twitch.ObjectMappers;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Database.Twitch.DataManagers;

public sealed class TwitchStreamerDataManager : ITwitchStreamerDataManager
{
    private readonly IDatabase _database;

    public TwitchStreamerDataManager(IDatabase database)
    {
        this._database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public ValueTask AddStreamerAsync(Streamer streamerName, int streamerId, DateTimeOffset startedStreaming, CancellationToken cancellationToken)
    {
        return this._database.ExecuteAsync(action: (c, ct) => TwitchStreamerObjectMapper.StreamerInsertAsync(connection: c,
                                                                                                             streamer: streamerName,
                                                                                                             id: streamerId,
                                                                                                             start_date: startedStreaming,
                                                                                                             cancellationToken: ct),
                                           cancellationToken: cancellationToken);
    }

    public ValueTask<TwitchUser?> GetByUserNameAsync(Streamer userName, CancellationToken cancellationToken)
    {
        return this._database.ExecuteAsync(action: (c, ct) => TwitchStreamerObjectMapper.StreamerGetAsync(connection: c, streamer: userName, cancellationToken: ct),
                                           cancellationToken: cancellationToken);
    }

    public ValueTask<TwitchUser?> GetByUserNameAsync(in Viewer userName, in CancellationToken cancellationToken)
    {
        return this.GetByUserNameAsync(userName.ToStreamer(), cancellationToken: cancellationToken);
    }
}