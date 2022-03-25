using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.StreamState;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

/// <summary>
///     Twitch stream online notification handler.
/// </summary>
public sealed class TwitchStreamOnlineNotificationHandler : INotificationHandler<TwitchStreamOnline>
{
    private readonly ILogger<TwitchStreamOnlineNotificationHandler> _logger;
    private readonly ITwitchChannelManager _twitchChannelManager;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="twitchChannelManager">Twitch Channel Manager.</param>
    /// <param name="logger">Logging</param>
    public TwitchStreamOnlineNotificationHandler(ITwitchChannelManager twitchChannelManager, ILogger<TwitchStreamOnlineNotificationHandler> logger)
    {
        this._twitchChannelManager = twitchChannelManager;
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task Handle(TwitchStreamOnline notification, CancellationToken cancellationToken)
    {
        this._logger.LogWarning($"{notification.Channel}: Started streaming \"{notification.Title}\" ({notification.GameName}) at {notification.StartedAt}");

        TwitchChannelState state = this._twitchChannelManager.GetChannel(notification.Channel);

        try
        {
            await state.OnlineAsync(gameName: notification.GameName, startDate: notification.StartedAt);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Channel}: Failed to notify Started streaming");
        }
    }
}