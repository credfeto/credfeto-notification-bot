using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchPaidReSubNotificationHandler : INotificationHandler<TwitchPaidReSub>
{
    private readonly IContributionThanks _contributionThanks;
    private readonly ILogger<TwitchPaidReSubNotificationHandler> _logger;

    public TwitchPaidReSubNotificationHandler(IContributionThanks contributionThanks, ILogger<TwitchPaidReSubNotificationHandler> logger)
    {
        this._contributionThanks = contributionThanks ?? throw new ArgumentNullException(nameof(contributionThanks));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(TwitchPaidReSub notification, CancellationToken cancellationToken)
    {
        try
        {
            await this._contributionThanks.ThankForPaidReSubAsync(streamer: notification.Streamer, user: notification.User, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Streamer}: Failed to notify re-sub (paid)");
        }
    }
}