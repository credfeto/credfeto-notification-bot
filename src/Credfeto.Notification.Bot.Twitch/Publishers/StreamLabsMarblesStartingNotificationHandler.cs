using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class CustomTriggeredMessageNotificationHandler : INotificationHandler<CustomTriggeredMessage>
{
    private readonly ILogger<CustomTriggeredMessageNotificationHandler> _logger;
    private readonly IMarblesJoiner _marblesJoiner;

    public CustomTriggeredMessageNotificationHandler(IMarblesJoiner marblesJoiner, ILogger<CustomTriggeredMessageNotificationHandler> logger)
    {
        this._marblesJoiner = marblesJoiner ?? throw new ArgumentNullException(nameof(marblesJoiner));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(CustomTriggeredMessage notification, CancellationToken cancellationToken)
    {
        try
        {
            await this._marblesJoiner.JoinMarblesAsync(streamer: notification.Streamer, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Streamer}: Failed to join marbles");
        }
    }
}