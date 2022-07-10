using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class MarblesStartingNotificationHandler : INotificationHandler<MarblesStarting>
{
    private readonly ILogger<MarblesStartingNotificationHandler> _logger;
    private readonly IMarblesJoiner _marblesJoiner;

    public MarblesStartingNotificationHandler(IMarblesJoiner marblesJoiner, ILogger<MarblesStartingNotificationHandler> logger)
    {
        this._marblesJoiner = marblesJoiner ?? throw new ArgumentNullException(nameof(marblesJoiner));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(MarblesStarting notification, CancellationToken cancellationToken)
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