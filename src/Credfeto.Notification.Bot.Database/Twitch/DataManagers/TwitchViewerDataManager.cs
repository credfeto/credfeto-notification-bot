using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Database;
using Credfeto.Notification.Bot.Database.Twitch.ObjectMappers;
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
        return this._database.ExecuteAsync(action: (c, ct) => TwitchViewerObjectMapper.ViewerInsertAsync(connection: c,
                                                                                                         viewer: viewerName,
                                                                                                         Convert.ToInt32(value: viewerId, provider: CultureInfo.InvariantCulture),
                                                                                                         dateCreated: dateCreated,
                                                                                                         cancellationToken: ct),
                                           cancellationToken: cancellationToken);
    }

    public ValueTask<TwitchUser?> GetByUserNameAsync(Viewer userName, CancellationToken cancellationToken)
    {
        return this._database.ExecuteAsync(action: (c, ct) => TwitchViewerObjectMapper.ViewerGetAsync(connection: c, viewer: userName, cancellationToken: ct), cancellationToken: cancellationToken);
    }
}