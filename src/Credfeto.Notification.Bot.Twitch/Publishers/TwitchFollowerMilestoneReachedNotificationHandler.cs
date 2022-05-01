using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Actions.Services;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.StreamState;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchFollowerMilestoneReachedNotificationHandler : MessageSenderBase, INotificationHandler<TwitchFollowerMilestoneReached>
{
    private readonly ILogger<TwitchFollowerMilestoneReachedNotificationHandler> _logger;
    private readonly ITwitchChannelManager _twitchChannelManager;

    public TwitchFollowerMilestoneReachedNotificationHandler(ITwitchChannelManager twitchChannelManager,
                                                             IMessageChannel<TwitchChatMessage> twitchChatMessageChannel,
                                                             ILogger<TwitchFollowerMilestoneReachedNotificationHandler> logger)
        : base(twitchChatMessageChannel)
    {
        this._twitchChannelManager = twitchChannelManager ?? throw new ArgumentNullException(nameof(twitchChannelManager));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(TwitchFollowerMilestoneReached notification, CancellationToken cancellationToken)
    {
        ITwitchChannelState channelState = this._twitchChannelManager.GetStreamer(notification.Streamer);

        if (channelState.Settings.AnnounceMilestonesEnabled)
        {
            return;
        }

        await this.SendMessageAsync(streamer: notification.Streamer, $"/me @{notification.Streamer} Woo! {notification.MilestoneReached} followers reached!", cancellationToken: cancellationToken);

        this._logger.LogWarning($"{notification.Streamer}: Woo!! New follower milestone reached {notification.MilestoneReached}");
    }
}