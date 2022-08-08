using System;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Database.Interfaces;
using Credfeto.Notification.Bot.Database.Interfaces.Builders;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Database.Twitch.DataManagers;

public sealed class TwitchViewerDataManager : ITwitchViewerDataManager
{
    private readonly IDatabase _database;
    private readonly IObjectBuilder<TwitchUserEntity, TwitchUser> _twitchUserBuilder;

    public TwitchViewerDataManager(IDatabase database, IObjectBuilder<TwitchUserEntity, TwitchUser> twitchUserBuilder)
    {
        this._database = database ?? throw new ArgumentNullException(nameof(database));
        this._twitchUserBuilder = twitchUserBuilder ?? throw new ArgumentNullException(nameof(twitchUserBuilder));
    }

    public Task AddViewerAsync(Viewer viewerName, string viewerId, DateTime dateCreated)
    {
        return this._database.ExecuteAsync(storedProcedure: "twitch.viewer_insert", new { username_ = viewerName.ToString(), id_ = viewerId, date_created_ = dateCreated });
    }

    public Task<TwitchUser?> GetByUserNameAsync(Viewer userName)
    {
        return this._database.QuerySingleOrDefaultAsync(builder: this._twitchUserBuilder, storedProcedure: "twitch.viewer_get", new { username_ = userName.ToString() });
    }
}