using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Database.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.DataTypes.Mappers;

namespace Credfeto.Notification.Bot.Database.Twitch.ObjectMappers;

internal static partial class TwitchViewerObjectMapper
{
    [SqlObjectMap(name: "twitch.viewer_insert", sqlObjectType: SqlObjectType.STORED_PROCEDURE)]
    public static partial ValueTask ViewerInsertAsync(DbConnection connection,
                                                      [SqlFieldMap<ViewerMapper, Viewer>] Viewer viewer,
                                                      int id,
                                                      [SqlFieldMap<DateTimeOffsetMapper, DateTimeOffset>] DateTimeOffset dateCreated,
                                                      CancellationToken cancellationToken);

    [SqlObjectMap(name: "twitch.viewer_get", sqlObjectType: SqlObjectType.STORED_PROCEDURE)]
    public static partial ValueTask<TwitchUser?> ViewerGetAsync(DbConnection connection, [SqlFieldMap<ViewerMapper, Viewer>] Viewer viewer, CancellationToken cancellationToken);
}