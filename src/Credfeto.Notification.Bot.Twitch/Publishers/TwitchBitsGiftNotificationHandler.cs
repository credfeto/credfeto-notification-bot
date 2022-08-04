using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchBitsGiftNotificationHandler : INotificationHandler<TwitchBitsGift>
{
    private readonly IContributionThanks _contributionThanks;
    private readonly ILogger<TwitchBitsGiftNotificationHandler> _logger;

    public TwitchBitsGiftNotificationHandler(IContributionThanks contributionThanks, ILogger<TwitchBitsGiftNotificationHandler> logger)
    {
        this._contributionThanks = contributionThanks ?? throw new ArgumentNullException(nameof(contributionThanks));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(TwitchBitsGift notification, CancellationToken cancellationToken)
    {
        try
        {
            await this._contributionThanks.ThankForBitsAsync(streamer: notification.Streamer, user: notification.User, bitsGiven: notification.Bits, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Streamer}: Failed to notify new bits gift");
        }
    }
}