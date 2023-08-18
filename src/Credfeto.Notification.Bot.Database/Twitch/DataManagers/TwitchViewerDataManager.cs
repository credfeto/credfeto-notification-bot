using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Database;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Database.Twitch.DataManagers;

public sealed class TwitchViewerDataManager : ITwitchViewerDataManager
{
    private readonly IDatabase _database;

    public TwitchViewerDataManager(IDatabase database)
    {
        this._database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public ValueTask AddViewerAsync(Viewer viewerName, string viewerId, DateTimeOffset dateCreated, CancellationToken cancellationToken)
    {
        return this._database.ExecuteAsync(storedProcedure: "twitch.viewer_insert", new { username_ = viewerName.ToString(), id_ = viewerId, date_created_ = dateCreated });
    }

    public ValueTask<TwitchUser?> GetByUserNameAsync(Viewer userName, CancellationToken cancellationToken)
    {
        return this._database.QuerySingleOrDefaultAsync(builder: this._twitchUserBuilder, storedProcedure: "twitch.viewer_get", new { username_ = userName.ToString() });
    }
}