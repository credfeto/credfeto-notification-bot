using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Models.MediatorModels;
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
            await this._contributionThanks.ThankForNewPrimeSubAsync(channel: notification.Channel, user: notification.User, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Channel}: Failed to notify new sub (prime)");
        }
    }
}