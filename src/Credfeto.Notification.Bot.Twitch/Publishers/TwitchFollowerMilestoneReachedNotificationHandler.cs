using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Actions.Services;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.StreamState;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchFollowerMilestoneReachedNotificationHandler : MessageSenderBase, INotificationHandler<TwitchFollowerMilestoneReached>
{
    private readonly ILogger<TwitchFollowerMilestoneReachedNotificationHandler> _logger;
    private readonly TwitchBotOptions _options;

    public TwitchFollowerMilestoneReachedNotificationHandler(IOptions<TwitchBotOptions> options,
                                                             IMessageChannel<TwitchChatMessage> twitchChatMessageChannel,
                                                             ILogger<TwitchFollowerMilestoneReachedNotificationHandler> logger)
        : base(twitchChatMessageChannel)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task Handle(TwitchFollowerMilestoneReached notification, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(notification.Streamer);

        if (modChannel?.MileStones.Enabled != true)
        {
            return;
        }

        await this.SendMessageAsync(streamer: notification.Streamer, $"/me @{notification.Streamer} Woo! {notification.MilestoneReached} followers reached!", cancellationToken: cancellationToken);

        this._logger.LogWarning($"{notification.Streamer}: Woo!! New follower milestone reached {notification.MilestoneReached}");
    }
}