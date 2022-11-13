using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Models;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class CustomTriggeredMessageNotificationHandler : INotificationHandler<CustomTriggeredMessage>
{
    private readonly ICustomTriggeredMessageSender _customTriggeredMessageSender;
    private readonly ILogger<CustomTriggeredMessageNotificationHandler> _logger;

    public CustomTriggeredMessageNotificationHandler(ICustomTriggeredMessageSender customTriggeredMessageSender, ILogger<CustomTriggeredMessageNotificationHandler> logger)
    {
        this._customTriggeredMessageSender = customTriggeredMessageSender ?? throw new ArgumentNullException(nameof(customTriggeredMessageSender));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask Handle(CustomTriggeredMessage notification, CancellationToken cancellationToken)
    {
        try
        {
            await this._customTriggeredMessageSender.SendAsync(streamer: notification.Streamer, message: notification.Message, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Streamer}: Failed to send custom message");
        }
    }
}