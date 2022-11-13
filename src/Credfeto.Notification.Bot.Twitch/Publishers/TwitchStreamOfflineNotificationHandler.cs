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

public sealed class TwitchStreamOfflineNotificationHandler : INotificationHandler<TwitchStreamOffline>
{
    private readonly ILogger<TwitchStreamOfflineNotificationHandler> _logger;
    private readonly TwitchBotOptions _options;
    private readonly ITwitchChannelManager _twitchChannelManager;

    public TwitchStreamOfflineNotificationHandler(IOptions<TwitchBotOptions> options,
                                                  ITwitchChannelManager twitchChannelManager,
                                                  ILogger<TwitchStreamOfflineNotificationHandler> logger)
    {
        this._twitchChannelManager = twitchChannelManager;
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
    }

    public ValueTask Handle(TwitchStreamOffline notification, CancellationToken cancellationToken)
    {
        this._logger.LogWarning($"{notification.Streamer}: Stopped streaming \"{notification.Title}\" ({notification.GameName}) at {notification.StartedAt}");

        if (!this._options.IsModChannel(notification.Streamer))
        {
            this._logger.LogDebug($"{notification.Streamer}: Ignoring non-mod channel");

            return ValueTask.CompletedTask;
        }

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(notification.Streamer);

        try
        {
            state.Offline();
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Streamer}: Failed to notify Started streaming");
        }

        return ValueTask.CompletedTask;
    }
}