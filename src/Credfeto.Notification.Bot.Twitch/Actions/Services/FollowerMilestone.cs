using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class FollowerMilestone : MessageSenderBase, IFollowerMilestone
{
    private readonly ILogger<FollowerMilestone> _logger;
    private readonly TwitchBotOptions _options;

    public FollowerMilestone(IOptions<TwitchBotOptions> options, IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ILogger<FollowerMilestone> logger)
        : base(twitchChatMessageChannel)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
    }

    public Task IssueMilestoneUpdateAsync(string channel, int followers, CancellationToken cancellationToken)
    {
        this._logger.LogWarning($"{channel}: Currently has {followers} followers");

        int[] orderedFollowers = this._options.Milestones.Followers.OrderBy(i => i)
                                     .ToArray();
        int lastMileStoneReached = orderedFollowers.LastOrDefault(f => f < followers);
        int nextMileStone = orderedFollowers.First(f => f > followers);

        double distance = nextMileStone - lastMileStoneReached;
        double left = followers - lastMileStoneReached;
        double progress = Math.Round(left / distance * 100, digits: 2);

        this._logger.LogWarning($"{channel}: Follower Milestone {lastMileStoneReached} Next {nextMileStone} Progress : {progress}% of gap filled");

        return Task.CompletedTask;
    }
}