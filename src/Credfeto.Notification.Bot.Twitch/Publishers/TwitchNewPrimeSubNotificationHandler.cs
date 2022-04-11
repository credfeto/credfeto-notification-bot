using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchNewPrimeSubNotificationHandler : INotificationHandler<TwitchNewPrimeSub>
{
    private readonly IContributionThanks _contributionThanks;
    private readonly ILogger<TwitchNewPrimeSubNotificationHandler> _logger;

    public TwitchNewPrimeSubNotificationHandler(IContributionThanks contributionThanks, ILogger<TwitchNewPrimeSubNotificationHandler> logger)
    {
        this._contributionThanks = contributionThanks ?? throw new ArgumentNullException(nameof(contributionThanks));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(TwitchNewPrimeSub notification, CancellationToken cancellationToken)
    {
        try
        {
            await this._contributionThanks.ThankForNewPrimeSubAsync(streamer: notification.Streamer, user: notification.User, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Streamer}: Failed to notify new sub (prime)");
        }
    }
}