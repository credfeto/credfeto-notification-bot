using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchChannelChatConnectedNotificationHandler : INotificationHandler<TwitchChannelChatConnected>
{
    private readonly IChannelFollowCount _channelFollowCount;
    private readonly IFollowerMilestone _followerMilestone;
    private readonly ILogger<TwitchChannelChatConnectedNotificationHandler> _logger;

    public TwitchChannelChatConnectedNotificationHandler(IChannelFollowCount channelFollowCount,
                                                         IFollowerMilestone followerMilestone,
                                                         ILogger<TwitchChannelChatConnectedNotificationHandler> logger)
    {
        this._channelFollowCount = channelFollowCount ?? throw new ArgumentNullException(nameof(channelFollowCount));
        this._followerMilestone = followerMilestone ?? throw new ArgumentNullException(nameof(followerMilestone));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(TwitchChannelChatConnected notification, CancellationToken cancellationToken)
    {
        try
        {
            int followers = await this._channelFollowCount.GetCurrentFollowerCountAsync(streamer: notification.Streamer, cancellationToken: cancellationToken);

            await this._followerMilestone.IssueMilestoneUpdateAsync(streamer: notification.Streamer, followers: followers, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Streamer}: Failed to notify chat connected");
        }
    }
}