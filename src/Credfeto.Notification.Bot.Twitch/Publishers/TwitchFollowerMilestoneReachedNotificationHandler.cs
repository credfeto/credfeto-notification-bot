using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Actions.Services;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.StreamState;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchFollowerMilestoneReachedNotificationHandler : MessageSenderBase, INotificationHandler<TwitchFollowerMilestoneReached>
{
    private readonly ILogger<TwitchFollowerMilestoneReachedNotificationHandler> _logger;

    public TwitchFollowerMilestoneReachedNotificationHandler(IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ILogger<TwitchFollowerMilestoneReachedNotificationHandler> logger)
        : base(twitchChatMessageChannel)
    {
        this._logger = logger;
    }

    public async Task Handle(TwitchFollowerMilestoneReached notification, CancellationToken cancellationToken)
    {
        await this.SendMessageAsync(channel: notification.Channel, $"/me @{notification.Channel} Woo! {notification.MilestoneReached} followers reached!", cancellationToken: cancellationToken);

        this._logger.LogWarning($"{notification.Channel}: Woo!! New follower milestone reached {notification.MilestoneReached}");
    }
}