using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Database.Interfaces;
using Credfeto.Notification.Bot.Database.Twitch.Mappers;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Database.Twitch.ObjectMappers;

public static partial class TwitchStreamObjectMapper
{
    [SqlObjectMap(name: "twitch.stream_insert", sqlObjectType: SqlObjectType.STORED_PROCEDURE)]
    public static partial ValueTask StreamInsertAsync(DbConnection connection,
                                                      [SqlFieldMap<StreamerMapper, Streamer>] Streamer channel,
                                                      [SqlFieldMap<DateTimeOffsetMapper, DateTimeOffset>] DateTimeOffset start_date,
                                                      CancellationToken cancellationToken);
}