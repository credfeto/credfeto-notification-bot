using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Database.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.DataTypes.Mappers;

namespace Credfeto.Notification.Bot.Database.Twitch.ObjectMappers;

internal static partial class TwitchStreamerObjectMapper
{
    [SqlObjectMap(name: "twitch.streamer_insert", sqlObjectType: SqlObjectType.STORED_PROCEDURE)]
    public static partial ValueTask StreamerInsertAsync(DbConnection connection,
                                                        [SqlFieldMap<StreamerMapper, Streamer>] Streamer streamer,
                                                        int id,
                                                        [SuppressMessage(category: "ReSharper", checkId: "InconsistentNaming", Justification = "Matches DB")]
                                                        [SqlFieldMap<DateTimeOffsetMapper, DateTimeOffset>]
                                                        DateTimeOffset start_date,
                                                        CancellationToken cancellationToken);

    [SqlObjectMap(name: "twitch.streamer_get", sqlObjectType: SqlObjectType.STORED_PROCEDURE)]
    public static partial ValueTask<TwitchUser?> StreamerGetAsync(DbConnection connection,
                                                                  [SqlFieldMap<StreamerMapper, Streamer>] Streamer streamer,
                                                                  CancellationToken cancellationToken);
}