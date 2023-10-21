using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class FollowerMilestone : IFollowerMilestone
{
    private readonly ILogger<FollowerMilestone> _logger;
    private readonly IMediator _mediator;
    private readonly TwitchBotOptions _options;
    private readonly ITwitchStreamDataManager _twitchStreamDataManager;

    public FollowerMilestone(IOptions<TwitchBotOptions> options, IMediator mediator, ITwitchStreamDataManager twitchStreamDataManager, ILogger<FollowerMilestone> logger)
    {
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this._twitchStreamDataManager = twitchStreamDataManager ?? throw new ArgumentNullException(nameof(twitchStreamDataManager));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
    }

    public async Task IssueMilestoneUpdateAsync(Streamer streamer, int followers, CancellationToken cancellationToken)
    {
        this._logger.LogWarning($"{streamer}: Currently has {followers} followers");

        int[] orderedFollowers = this._options.Milestones.Followers.OrderBy(i => i)
                                     .ToArray();
        int lastMileStoneReached = orderedFollowers.LastOrDefault(f => f < followers);
        int nextMileStone = orderedFollowers.First(f => f > followers);

        double distance = nextMileStone - lastMileStoneReached;
        double left = followers - lastMileStoneReached;
        double progress = Math.Round(left / distance * 100, digits: 2);

        this._logger.LogWarning($"{streamer}: Follower Milestone {lastMileStoneReached} Next {nextMileStone} Progress : {progress}% of gap filled");

        if (lastMileStoneReached == 0)
        {
            // New streamer - no need to do anything yet
            return;
        }

        bool milestoneFreshlyReached =
            await this._twitchStreamDataManager.UpdateFollowerMilestoneAsync(streamer: streamer, followerCount: lastMileStoneReached, cancellationToken: cancellationToken);

        if (milestoneFreshlyReached)
        {
            await this._mediator.Publish(
                new TwitchFollowerMilestoneReached(streamer: streamer, milestoneReached: lastMileStoneReached, nextMilestone: nextMileStone, progress: progress),
                cancellationToken: cancellationToken);
            this._logger.LogWarning($"{streamer}: Woo!! New follower milestone reached {lastMileStoneReached}");
        }
    }
}