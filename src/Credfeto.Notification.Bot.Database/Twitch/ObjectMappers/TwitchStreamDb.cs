using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Database.Interfaces;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;
using Credfeto.Notification.Bot.Database.Twitch.Mappers;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Database.Twitch.ObjectMappers;

internal static partial class TwitchStreamObjectMapper
{
    [SqlObjectMap(name: "twitch.stream_insert", sqlObjectType: SqlObjectType.STORED_PROCEDURE)]
    public static partial ValueTask StreamInsertAsync(DbConnection connection,
                                                      [SqlFieldMap<StreamerMapper, Streamer>] Streamer channel,
                                                      [SqlFieldMap<DateTimeOffsetMapper, DateTimeOffset>] DateTimeOffset start_date,
                                                      CancellationToken cancellationToken);

    [SqlObjectMap(name: "twitch.stream_chatter_insert", sqlObjectType: SqlObjectType.STORED_PROCEDURE)]
    public static partial ValueTask StreamChatterInsertAsync(DbConnection connection,
                                                             [SqlFieldMap<StreamerMapper, Streamer>] Streamer channel,
                                                             [SqlFieldMap<DateTimeOffsetMapper, DateTimeOffset>] DateTimeOffset start_date,
                                                             [SqlFieldMap<ViewerMapper, Viewer>] Viewer viewer,
                                                             CancellationToken cancellationToken);

    [SqlObjectMap(name: "twitch.stream_settings_set", sqlObjectType: SqlObjectType.STORED_PROCEDURE)]
    public static partial ValueTask StreamSettingsSetAsync(DbConnection dbConnection,
                                                           [SqlFieldMap<StreamerMapper, Streamer>] Streamer channel,
                                                           [SqlFieldMap<DateTimeOffsetMapper, DateTimeOffset>] DateTimeOffset start_date,
                                                           bool announce_milestones,
                                                           bool chat_welcomes,
                                                           bool raid_welcomes,
                                                           bool shout_outs,
                                                           CancellationToken cancellationToken);

    [SqlObjectMap(name: "twitch.stream_chatter_get", sqlObjectType: SqlObjectType.TABLE_FUNCTION)]
    public static partial ValueTask<TwitchChatter> StreamChatterGetAsync(DbConnection connection, Streamer channel, DateTimeOffset start_date, Viewer viewer, CancellationToken cancellationToken);
}