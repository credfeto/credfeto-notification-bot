using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.StreamState;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

/// <summary>
///     Twitch stream offline notification handler.
/// </summary>
public sealed class TwitchStreamOfflineNotificationHandler : INotificationHandler<TwitchStreamOffline>
{
    private readonly ILogger<TwitchStreamOfflineNotificationHandler> _logger;
    private readonly ITwitchChannelManager _twitchChannelManager;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="twitchChannelManager">Twitch Channel Manager.</param>
    /// <param name="logger">Logging</param>
    public TwitchStreamOfflineNotificationHandler(ITwitchChannelManager twitchChannelManager, ILogger<TwitchStreamOfflineNotificationHandler> logger)
    {
        this._twitchChannelManager = twitchChannelManager;
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public Task Handle(TwitchStreamOffline notification, CancellationToken cancellationToken)
    {
        this._logger.LogWarning($"{notification.Channel}: Stopped streaming \"{notification.Title}\" ({notification.GameName}) at {notification.StartedAt}");

        TwitchChannelState state = this._twitchChannelManager.GetChannel(notification.Channel);

        try
        {
            state.Offline();
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Channel}: Failed to notify Started streaming");
        }

        return Task.CompletedTask;
    }
}