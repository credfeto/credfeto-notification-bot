using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Models;
using Mediator;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchIncomingMessageNotificationHandler : INotificationHandler<TwitchIncomingMessage>
{
    private readonly ITwitchCustomMessageHandler _twitchCustomMessageHandler;

    public TwitchIncomingMessageNotificationHandler(ITwitchCustomMessageHandler twitchCustomMessageHandler)
    {
        this._twitchCustomMessageHandler =
            twitchCustomMessageHandler ?? throw new ArgumentNullException(nameof(twitchCustomMessageHandler));
    }

    public async ValueTask Handle(TwitchIncomingMessage notification, CancellationToken cancellationToken)
    {
        bool handled = await this._twitchCustomMessageHandler.HandleMessageAsync(
            message: notification,
            cancellationToken: cancellationToken
        );

        if (handled)
        {
            Debug.WriteLine("Message handled by custom handler");
        }
    }
}
