using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Models;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchNewPaidSubNotificationHandler : INotificationHandler<TwitchNewPaidSub>
{
    private readonly IContributionThanks _contributionThanks;
    private readonly ILogger<TwitchNewPaidSubNotificationHandler> _logger;

    public TwitchNewPaidSubNotificationHandler(IContributionThanks contributionThanks, ILogger<TwitchNewPaidSubNotificationHandler> logger)
    {
        this._contributionThanks = contributionThanks ?? throw new ArgumentNullException(nameof(contributionThanks));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask Handle(TwitchNewPaidSub notification, CancellationToken cancellationToken)
    {
        try
        {
            await this._contributionThanks.ThankForNewPaidSubAsync(streamer: notification.Streamer, user: notification.User, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Streamer}: Failed to notify new sub (paid)");
        }
    }
}