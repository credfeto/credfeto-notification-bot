using System;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Database.Interfaces;
using Credfeto.Notification.Bot.Database.Interfaces.Builders;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Database.Twitch.DataManagers;

public sealed class TwitchStreamDataManager : ITwitchStreamDataManager
{
    private readonly IObjectBuilder<TwitchChatterEntity, TwitchChatter> _chatterBuilder;
    private readonly IDatabase _database;
    private readonly IObjectBuilder<TwitchFollowerEntity, TwitchFollower> _followerBuilder;
    private readonly IObjectBuilder<TwitchFollowerMilestoneEntity, TwitchFollowerMilestone> _followerMilestoneBuilder;
    private readonly IObjectBuilder<TwitchRegularChatterEntity, TwitchRegularChatter> _regularChatterBuilder;

    public TwitchStreamDataManager(IDatabase database,
                                   IObjectBuilder<TwitchChatterEntity, TwitchChatter> chatterBuilder,
                                   IObjectBuilder<TwitchRegularChatterEntity, TwitchRegularChatter> regularChatterBuilder,
                                   IObjectBuilder<TwitchFollowerMilestoneEntity, TwitchFollowerMilestone> followerMilestoneBuilder,
                                   IObjectBuilder<TwitchFollowerEntity, TwitchFollower> followerBuilder)
    {
        this._database = database ?? throw new ArgumentNullException(nameof(database));
        this._chatterBuilder = chatterBuilder ?? throw new ArgumentNullException(nameof(chatterBuilder));
        this._regularChatterBuilder = regularChatterBuilder ?? throw new ArgumentNullException(nameof(regularChatterBuilder));
        this._followerMilestoneBuilder = followerMilestoneBuilder ?? throw new ArgumentNullException(nameof(followerMilestoneBuilder));
        this._followerBuilder = followerBuilder ?? throw new ArgumentNullException(nameof(followerBuilder));
    }

    public Task RecordStreamStartAsync(Channel channel, DateTime streamStartDate)
    {
        return this._database.ExecuteAsync(storedProcedure: "twitch.stream_insert", new { channel_ = channel.ToString(), start_date_ = streamStartDate });
    }

    public Task AddChatterToStreamAsync(Channel channel, DateTime streamStartDate, User username)
    {
        return this._database.ExecuteAsync(storedProcedure: "twitch.stream_chatter_insert", new { channel_ = channel.ToString(), start_date_ = streamStartDate, chat_user_ = username.ToString() });
    }

    public async Task<bool> IsFirstMessageInStreamAsync(Channel channel, DateTime streamStartDate, User username)
    {
        TwitchChatter? chatted = await this._database.QuerySingleOrDefaultAsync(builder: this._chatterBuilder,
                                                                                storedProcedure: "twitch.stream_chatter_get",
                                                                                new { channel_ = channel.ToString(), start_date_ = streamStartDate, chat_user_ = username.ToString() });

        return chatted == null;
    }

    public async Task<bool> IsRegularChatterAsync(Channel channel, User username)
    {
        TwitchRegularChatter? chatted = await this._database.QuerySingleOrDefaultAsync(builder: this._regularChatterBuilder,
                                                                                       storedProcedure: "twitch.stream_chatter_is_regular",
                                                                                       new { channel_ = channel.ToString(), username_ = username.ToString() });

        return chatted?.Regular == true;
    }

    public async Task<bool> UpdateFollowerMilestoneAsync(Channel channel, int followerCount)
    {
        TwitchFollowerMilestone? milestone = await this._database.QuerySingleOrDefaultAsync(builder: this._followerMilestoneBuilder,
                                                                                            storedProcedure: "twitch.stream_milestone_insert",
                                                                                            new { channel_ = channel.ToString(), followers_ = followerCount });

        return milestone?.FreshlyReached == true;
    }

    public async Task<int> RecordNewFollowerAsync(Channel channel, User username)
    {
        TwitchFollower follower = await this._database.QuerySingleAsync(builder: this._followerBuilder,
                                                                        storedProcedure: "twitch.stream_follower_insert",
                                                                        new { channel_ = channel.ToString(), follower_ = username.ToString() });

        return follower.FollowCount;
    }
}