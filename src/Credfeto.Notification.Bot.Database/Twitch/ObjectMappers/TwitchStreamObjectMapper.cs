using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Database.Interfaces;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.DataTypes.Mappers;

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
    public static partial ValueTask StreamSettingsSetAsync(DbConnection connection,
                                                           [SqlFieldMap<StreamerMapper, Streamer>] Streamer channel,
                                                           [SqlFieldMap<DateTimeOffsetMapper, DateTimeOffset>] DateTimeOffset start_date,
                                                           bool announce_milestones,
                                                           bool chat_welcomes,
                                                           bool raid_welcomes,
                                                           bool shout_outs,
                                                           CancellationToken cancellationToken);

    [SqlObjectMap(name: "twitch.stream_chatter_get", sqlObjectType: SqlObjectType.TABLE_FUNCTION)]
    public static partial ValueTask<IReadOnlyList<TwitchChatter>> StreamChatterGetAsync(DbConnection connection,
                                                                                        [SqlFieldMap<StreamerMapper, Streamer>] Streamer channel,
                                                                                        [SqlFieldMap<DateTimeOffsetMapper, DateTimeOffset>] DateTimeOffset start_date,
                                                                                        [SqlFieldMap<ViewerMapper, Viewer>] Viewer viewer,
                                                                                        CancellationToken cancellationToken);

    [SqlObjectMap(name: "twitch.stream_chatter_is_regular", sqlObjectType: SqlObjectType.TABLE_FUNCTION)]
    public static partial ValueTask<TwitchRegularChatter?> StreamChatterIsRegularAsync(DbConnection connection,
                                                                                       [SqlFieldMap<StreamerMapper, Streamer>] Streamer channel,
                                                                                       [SqlFieldMap<ViewerMapper, Viewer>] Viewer viewer,
                                                                                       CancellationToken cancellationToken);

    [SqlObjectMap(name: "twitch.stream_milestone_insert", sqlObjectType: SqlObjectType.TABLE_FUNCTION)]
    public static partial ValueTask<IReadOnlyList<TwitchFollowerMilestone>> StreamFollowerMilestoneInsertAsync(DbConnection connection,
                                                                                                               [SqlFieldMap<StreamerMapper, Streamer>] Streamer channel,
                                                                                                               int followers,
                                                                                                               CancellationToken cancellationToken);

    [SqlObjectMap(name: "twitch.stream_follower_insert", sqlObjectType: SqlObjectType.STORED_PROCEDURE)]
    public static partial ValueTask<IReadOnlyList<TwitchFollower>> StreamFollowerInsertAsync(DbConnection connection,
                                                                                             [SqlFieldMap<StreamerMapper, Streamer>] Streamer channel,
                                                                                             [SqlFieldMap<ViewerMapper, Viewer>] Viewer viewer,
                                                                                             CancellationToken cancellationToken);

    [SqlObjectMap(name: "twitch.stream_settings_get", sqlObjectType: SqlObjectType.STORED_PROCEDURE)]
    public static partial ValueTask<IReadOnlyList<StreamSettings>> StreamSettingsGetAsync(DbConnection connection,
                                                                                          [SqlFieldMap<StreamerMapper, Streamer>] Streamer channel,
                                                                                          [SqlFieldMap<DateTimeOffsetMapper, DateTimeOffset>] DateTimeOffset start_date,
                                                                                          CancellationToken cancellationToken);
}