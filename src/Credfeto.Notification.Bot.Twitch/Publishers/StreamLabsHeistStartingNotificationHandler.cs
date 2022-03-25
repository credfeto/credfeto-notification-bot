using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Models.MediatorModels;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class StreamLabsHeistStartingNotificationHandler : INotificationHandler<StreamLabsHeistStarting>
{
    private readonly IHeistJoiner _heistJoiner;
    private readonly ILogger<StreamLabsHeistStartingNotificationHandler> _logger;

    public StreamLabsHeistStartingNotificationHandler(IHeistJoiner heistJoiner, ILogger<StreamLabsHeistStartingNotificationHandler> logger)
    {
        this._heistJoiner = heistJoiner ?? throw new ArgumentNullException(nameof(heistJoiner));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(StreamLabsHeistStarting notification, CancellationToken cancellationToken)
    {
        try
        {
            await this._heistJoiner.JoinHeistAsync(channel: notification.Channel, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Channel}: Failed to join heist");
        }
    }
}