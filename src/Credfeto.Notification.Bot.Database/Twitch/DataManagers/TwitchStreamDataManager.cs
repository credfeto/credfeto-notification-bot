using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Database;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;
using Credfeto.Notification.Bot.Database.Twitch.ObjectMappers;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Database.Twitch.DataManagers;

public sealed class TwitchStreamDataManager : ITwitchStreamDataManager
{
    private readonly IDatabase _database;

    public TwitchStreamDataManager(IDatabase database)
    {
        this._database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public ValueTask RecordStreamStartAsync(Streamer streamer, DateTimeOffset streamStartDate, CancellationToken cancellationToken)
    {
        return this._database.ExecuteAsync(action: (c, ct) => TwitchStreamObjectMapper.StreamInsertAsync(connection: c, channel: streamer, start_date: streamStartDate, cancellationToken: ct),
                                           cancellationToken: cancellationToken);
    }

    public ValueTask AddChatterToStreamAsync(Streamer streamer, DateTimeOffset streamStartDate, Viewer username, CancellationToken cancellationToken)
    {
        return this._database.ExecuteAsync(action: (c, ct) =>
                                                       TwitchStreamObjectMapper.StreamChatterInsertAsync(connection: c,
                                                                                                         channel: streamer,
                                                                                                         start_date: streamStartDate,
                                                                                                         viewer: username,
                                                                                                         cancellationToken: ct),
                                           cancellationToken: cancellationToken);
    }

    public async ValueTask<bool> IsFirstMessageInStreamAsync(Streamer streamer, DateTimeOffset streamStartDate, Viewer username, CancellationToken cancellationToken)
    {
        return (await this._database.ExecuteAsync(action: (c, ct) =>
                                                              TwitchStreamObjectMapper.StreamChatterGetAsync(connection: c,
                                                                                                             channel: streamer,
                                                                                                             start_date: streamStartDate,
                                                                                                             viewer: username,
                                                                                                             cancellationToken: ct),
                                                  cancellationToken: cancellationToken)).Count != 0;

        //                                    TwitchChatter? chatted = await this._database.QuerySingleOrDefaultAsync(builder: this._chatterBuilder,
        //                                                                                                            storedProcedure: "twitch.stream_chatter_get",
        //                                                                                                            new { channel_ = streamer.ToString(), start_date_ = streamStartDate, chat_user_ = username.ToString() });
        //
        // return chatted == null;
    }

    public async ValueTask<bool> IsRegularChatterAsync(Streamer streamer, Viewer username, CancellationToken cancellationToken)
    {
        TwitchRegularChatter? chatted = await this._database.QuerySingleOrDefaultAsync(builder: this._regularChatterBuilder,
                                                                                       storedProcedure: "twitch.stream_chatter_is_regular",
                                                                                       new { channel_ = streamer.ToString(), username_ = username.ToString() });

        return chatted?.Regular == true;
    }

    public async ValueTask<bool> UpdateFollowerMilestoneAsync(Streamer streamer, int followerCount, CancellationToken cancellationToken)
    {
        TwitchFollowerMilestone? milestone = await this._database.QuerySingleOrDefaultAsync(builder: this._followerMilestoneBuilder,
                                                                                            storedProcedure: "twitch.stream_milestone_insert",
                                                                                            new { channel_ = streamer.ToString(), followers_ = followerCount });

        return milestone?.FreshlyReached == true;
    }

    public async ValueTask<int> RecordNewFollowerAsync(Streamer streamer, Viewer username, CancellationToken cancellationToken)
    {
        TwitchFollower follower = await this._database.QuerySingleAsync(builder: this._followerBuilder,
                                                                        storedProcedure: "twitch.stream_follower_insert",
                                                                        new { channel_ = streamer.ToString(), follower_ = username.ToString() });

        return follower.FollowCount;
    }

    public ValueTask<StreamSettings?> GetSettingsAsync(Streamer streamer, DateTimeOffset streamStartDate, CancellationToken cancellationToken)
    {
        return this._database.QuerySingleOrDefaultAsync(builder: this._streamSettingsBuilder,
                                                        storedProcedure: "twitch.stream_settings_get",
                                                        new { channel_ = streamer.ToString(), start_date_ = streamStartDate });
    }

    public ValueTask UpdateSettingsAsync(Streamer streamer, DateTimeOffset streamStartDate, StreamSettings settings, CancellationToken cancellationToken)
    {
        return this._database.ExecuteAsync(action: (c, ct) => TwitchStreamObjectMapper.StreamSettingsSetAsync(dbConnection: c,
                                                                                                              channel: streamer,
                                                                                                              start_date: streamStartDate,
                                                                                                              announce_milestones: settings.AnnounceMilestonesEnabled,
                                                                                                              chat_welcomes: settings.ChatWelcomesEnabled,
                                                                                                              raid_welcomes: settings.RaidWelcomesEnabled,
                                                                                                              shout_outs: settings.ShoutOutsEnabled,
                                                                                                              cancellationToken: ct),
                                           cancellationToken: cancellationToken);
    }
}