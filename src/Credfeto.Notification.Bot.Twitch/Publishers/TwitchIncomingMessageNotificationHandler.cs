using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchIncomingMessageNotificationHandler : INotificationHandler<TwitchIncomingMessage>
{
    private readonly ILogger<TwitchIncomingMessageNotificationHandler> _logger;
    private readonly TwitchBotOptions _options;
    private readonly ITwitchChannelManager _twitchChannelManager;
    private readonly ITwitchCustomMessageHandler _twitchCustomMessageHandler;

    public TwitchIncomingMessageNotificationHandler(IOptions<TwitchBotOptions> options,
                                                    ITwitchChannelManager twitchChannelManager,
                                                    ITwitchCustomMessageHandler twitchCustomMessageHandler,
                                                    ILogger<TwitchIncomingMessageNotificationHandler> logger)
    {
        this._twitchChannelManager = twitchChannelManager ?? throw new ArgumentNullException(nameof(twitchChannelManager));
        this._twitchCustomMessageHandler = twitchCustomMessageHandler ?? throw new ArgumentNullException(nameof(twitchCustomMessageHandler));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
    }

    public async ValueTask Handle(TwitchIncomingMessage notification, CancellationToken cancellationToken)
    {
        bool handled = await this._twitchCustomMessageHandler.HandleMessageAsync(message: notification, cancellationToken: cancellationToken);

        if (handled)
        {
            return;
        }

        if (!this._options.IsModChannel(notification.Streamer))
        {
            return;
        }

        this._logger.LogInformation($"{notification.Streamer.Value}: @{notification.Chatter.Value}: {notification.Message}");

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(notification.Streamer);

        await state.ChatMessageAsync(user: notification.Chatter, message: notification.Message, cancellationToken: cancellationToken);
    }
}