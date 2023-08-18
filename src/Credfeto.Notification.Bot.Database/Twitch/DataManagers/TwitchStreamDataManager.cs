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
    }

    public async ValueTask<bool> IsRegularChatterAsync(Streamer streamer, Viewer username, CancellationToken cancellationToken)
    {
        TwitchRegularChatter? chatted = await this._database.ExecuteAsync(
            action: (c, ct) => TwitchStreamObjectMapper.StreamChatterIsRegularAsync(connection: c, channel: streamer, viewer: username, cancellationToken: ct),
            cancellationToken: cancellationToken);

        return chatted?.IsRegular == true;
    }

    public async ValueTask<bool> UpdateFollowerMilestoneAsync(Streamer streamer, int followerCount, CancellationToken cancellationToken)
    {
        TwitchFollowerMilestone? milestone = (await this._database.ExecuteAsync(
            action: (c, ct) => TwitchStreamObjectMapper.StreamFollowerMilestoneInsertAsync(connection: c, channel: streamer, followers: followerCount, cancellationToken: ct),
            cancellationToken: cancellationToken)).FirstOrDefault();

        return milestone?.FreshlyReached == true;
    }

    public async ValueTask<int> RecordNewFollowerAsync(Streamer streamer, Viewer username, CancellationToken cancellationToken)
    {
        TwitchFollower? follower = (await this._database.ExecuteAsync(
            action: (c, ct) => TwitchStreamObjectMapper.StreamFollowerInsertAsync(connection: c, channel: streamer, viewer: username, cancellationToken: ct),
            cancellationToken: cancellationToken)).FirstOrDefault();

        return follower?.FollowCount ?? 0;
    }

    public async ValueTask<StreamSettings?> GetSettingsAsync(Streamer streamer, DateTimeOffset streamStartDate, CancellationToken cancellationToken)
    {
        return (await this._database.ExecuteAsync(
            action: (c, ct) => TwitchStreamObjectMapper.StreamSettingsGetAsync(connection: c, channel: streamer, start_date: streamStartDate, cancellationToken: ct),
            cancellationToken: cancellationToken)).FirstOrDefault();
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