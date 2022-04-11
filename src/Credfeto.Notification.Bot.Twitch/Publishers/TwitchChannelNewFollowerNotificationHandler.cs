using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

/// <summary>
///     Twitch channel new follower notification handler.
/// </summary>
public sealed class TwitchChannelNewFollowerNotificationHandler : INotificationHandler<TwitchChannelNewFollower>
{
    private readonly IChannelFollowCount _channelFollowCount;
    private readonly IFollowerMilestone _followerMilestone;
    private readonly ILogger<TwitchChannelNewFollowerNotificationHandler> _logger;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="channelFollowCount">Follower count check</param>
    /// <param name="followerMilestone">Follower milestone checker</param>
    /// <param name="logger">Logging</param>
    public TwitchChannelNewFollowerNotificationHandler(IChannelFollowCount channelFollowCount, IFollowerMilestone followerMilestone, ILogger<TwitchChannelNewFollowerNotificationHandler> logger)
    {
        this._channelFollowCount = channelFollowCount;
        this._followerMilestone = followerMilestone;
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task Handle(TwitchChannelNewFollower notification, CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Check DB to see if user has ever followed
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