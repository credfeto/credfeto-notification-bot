using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

/// <summary>
///     Twitch stream online notification handler.
/// </summary>
public sealed class TwitchStreamOnlineNotificationHandler : INotificationHandler<TwitchStreamOnline>
{
    private readonly ILogger<TwitchStreamOnlineNotificationHandler> _logger;
    private readonly TwitchBotOptions _options;
    private readonly ITwitchChannelManager _twitchChannelManager;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="options">Twitch Bot options</param>
    /// <param name="twitchChannelManager">Twitch Channel Manager.</param>
    /// <param name="logger">Logging</param>
    public TwitchStreamOnlineNotificationHandler(IOptions<TwitchBotOptions> options, ITwitchChannelManager twitchChannelManager, ILogger<TwitchStreamOnlineNotificationHandler> logger)
    {
        this._twitchChannelManager = twitchChannelManager;
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
    }

    /// <inheritdoc />
    public async Task Handle(TwitchStreamOnline notification, CancellationToken cancellationToken)
    {
        this._logger.LogWarning($"{notification.Streamer}: Started streaming \"{notification.Title}\" ({notification.GameName}) at {notification.StartedAt}");

        if (!this._options.IsModChannel(notification.Streamer))
        {
            this._logger.LogDebug($"{notification.Streamer}: Ignoring non-mod channel");

            return;
        }

        ITwitchChannelState state = this._twitchChannelManager.GetStreamer(notification.Streamer);

        try
        {
            await state.OnlineAsync(gameName: notification.GameName, startDate: notification.StartedAt);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Streamer}: Failed to notify Started streaming");
        }
    }
}