using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchGiftSubSingleNotificationHandler : INotificationHandler<TwitchGiftSubSingle>
{
    private readonly IContributionThanks _contributionThanks;
    private readonly ILogger<TwitchGiftSubSingleNotificationHandler> _logger;

    public TwitchGiftSubSingleNotificationHandler(IContributionThanks contributionThanks, ILogger<TwitchGiftSubSingleNotificationHandler> logger)
    {
        this._contributionThanks = contributionThanks ?? throw new ArgumentNullException(nameof(contributionThanks));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(TwitchGiftSubSingle notification, CancellationToken cancellationToken)
    {
        try
        {
            await this._contributionThanks.ThankForGiftingSubAsync(streamer: notification.Streamer, giftedBy: notification.User, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Streamer}: Failed to notify gift sub (single)");
        }
    }
}