using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchGiftSubMultipleNotificationHandler : INotificationHandler<TwitchGiftSubMultiple>
{
    private readonly IContributionThanks _contributionThanks;
    private readonly ILogger<TwitchGiftSubMultipleNotificationHandler> _logger;

    public TwitchGiftSubMultipleNotificationHandler(IContributionThanks contributionThanks, ILogger<TwitchGiftSubMultipleNotificationHandler> logger)
    {
        this._contributionThanks = contributionThanks ?? throw new ArgumentNullException(nameof(contributionThanks));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(TwitchGiftSubMultiple notification, CancellationToken cancellationToken)
    {
        try
        {
            await this._contributionThanks.ThankForMultipleGiftSubsAsync(streamer: notification.Streamer, giftedBy: notification.User, count: notification.Count, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Streamer}: Failed to notify gift sub (multiple)");
        }
    }
}