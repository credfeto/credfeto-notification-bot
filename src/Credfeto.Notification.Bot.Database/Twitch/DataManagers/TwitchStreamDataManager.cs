using System;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Database.Interfaces;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;

namespace Credfeto.Notification.Bot.Database.Twitch.DataManagers;

public sealed class TwitchStreamDataManager : ITwitchStreamDataManager
{
    private readonly IDatabase _database;

    public TwitchStreamDataManager(IDatabase database)
    {
        this._database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public Task RecordStreamStartAsync(string channel, DateTime streamStartDate)
    {
        return this._database.ExecuteAsync(storedProcedure: "twitch.streamer_insert", new { username_ = channel, started_streaming_ = streamStartDate });
    }
}