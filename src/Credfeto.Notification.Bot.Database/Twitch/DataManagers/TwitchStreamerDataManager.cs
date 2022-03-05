using System;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Database.Interfaces;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;

namespace Credfeto.Notification.Bot.Database.Twitch.DataManagers;

public sealed class TwitchStreamerDataManager : ITwitchStreamerDataManager
{
    private readonly IDatabase _database;

    public TwitchStreamerDataManager(IDatabase database)
    {
        this._database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public Task AddStreamerAsync(string streamerName, DateTime startedStreaming)
    {
        return this._database.ExecuteAsync(storedProcedure: "twitch.streamer_insert", new { username_ = streamerName, started_streaming_ = startedStreaming });
    }

    public Task<TwitchUser?> GetByUserNameAsync(string userName)
    {
        throw new NotImplementedException();
    }
}