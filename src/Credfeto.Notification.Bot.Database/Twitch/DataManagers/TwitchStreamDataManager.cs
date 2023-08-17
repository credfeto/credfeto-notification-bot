using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Database;
using Credfeto.Database.Interfaces;
using Credfeto.Database.Interfaces.Builders;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;
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
        return this._database.ExecuteAsync(action: (c, ct) => TwitchStreamObjectMapper.StreamInsertAsync(c, channel: streamer, start_date: streamStartDate, ct), cancellationToken);
    }

    public Task AddChatterToStreamAsync(Streamer streamer, DateTimeOffset streamStartDate, Viewer username)
    {
        return this._database.ExecuteAsync(storedProcedure: "twitch.stream_chatter_insert", new { channel_ = streamer.ToString(), start_date_ = streamStartDate, chat_user_ = username.ToString() });
    }

    public async Task<bool> IsFirstMessageInStreamAsync(Streamer streamer, DateTimeOffset streamStartDate, Viewer username)
    {
        TwitchChatter? chatted = await this._database.QuerySingleOrDefaultAsync(builder: this._chatterBuilder,
                                                                                storedProcedure: "twitch.stream_chatter_get",
                                                                                new { channel_ = streamer.ToString(), start_date_ = streamStartDate, chat_user_ = username.ToString() });

        return chatted == null;
    }

    public async Task<bool> IsRegularChatterAsync(Streamer streamer, Viewer username)
    {
        TwitchRegularChatter? chatted = await this._database.QuerySingleOrDefaultAsync(builder: this._regularChatterBuilder,
                                                                                       storedProcedure: "twitch.stream_chatter_is_regular",
                                                                                       new { channel_ = streamer.ToString(), username_ = username.ToString() });

        return chatted?.Regular == true;
    }

    public async Task<bool> UpdateFollowerMilestoneAsync(Streamer streamer, int followerCount)
    {
        TwitchFollowerMilestone? milestone = await this._database.QuerySingleOrDefaultAsync(builder: this._followerMilestoneBuilder,
                                                                                            storedProcedure: "twitch.stream_milestone_insert",
                                                                                            new { channel_ = streamer.ToString(), followers_ = followerCount });

        return milestone?.FreshlyReached == true;
    }

    public async Task<int> RecordNewFollowerAsync(Streamer streamer, Viewer username)
    {
        TwitchFollower follower = await this._database.QuerySingleAsync(builder: this._followerBuilder,
                                                                        storedProcedure: "twitch.stream_follower_insert",
                                                                        new { channel_ = streamer.ToString(), follower_ = username.ToString() });

        return follower.FollowCount;
    }

    public Task<StreamSettings?> GetSettingsAsync(Streamer streamer, DateTimeOffset streamStartDate)
    {
        return this._database.QuerySingleOrDefaultAsync(builder: this._streamSettingsBuilder,
                                                        storedProcedure: "twitch.stream_settings_get",
                                                        new { channel_ = streamer.ToString(), start_date_ = streamStartDate });
    }

    public Task UpdateSettingsAsync(Streamer streamer, DateTimeOffset streamStartDate, StreamSettings settings)
    {
        return this._database.ExecuteAsync(storedProcedure: "twitch.stream_settings_set",
                                           new
                                           {
                                               channel_ = streamer.ToString(),
                                               start_date_ = streamStartDate,
                                               thanks_ = settings.ThanksEnabled,
                                               announce_milestones_ = settings.AnnounceMilestonesEnabled,
                                               chat_welcomes_ = settings.ChatWelcomesEnabled,
                                               raid_welcomes_ = settings.RaidWelcomesEnabled,
                                               shout_outs_ = settings.ShoutOutsEnabled
                                           });
    }
}