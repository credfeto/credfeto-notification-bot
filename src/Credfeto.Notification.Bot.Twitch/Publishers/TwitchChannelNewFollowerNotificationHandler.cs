using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchChannelNewFollowerNotificationHandler : INotificationHandler<TwitchChannelNewFollower>
{
    private readonly IChannelFollowCount _channelFollowCount;
    private readonly IFollowerMilestone _followerMilestone;
    private readonly ILogger<TwitchChannelNewFollowerNotificationHandler> _logger;

    public TwitchChannelNewFollowerNotificationHandler(IChannelFollowCount channelFollowCount,
                                                       IFollowerMilestone followerMilestone,
                                                       ILogger<TwitchChannelNewFollowerNotificationHandler> logger)
    {
        this._channelFollowCount = channelFollowCount;
        this._followerMilestone = followerMilestone;
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask Handle(TwitchChannelNewFollower notification, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken: cancellationToken);

            int followers = await this._channelFollowCount.GetCurrentFollowerCountAsync(streamer: notification.Streamer, cancellationToken: cancellationToken);

            await this._followerMilestone.IssueMilestoneUpdateAsync(streamer: notification.Streamer, followers: followers, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Streamer}: Failed to notify Started streaming");
        }
    }
}