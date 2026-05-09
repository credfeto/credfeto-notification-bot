using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers.LoggingExtensions;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchWatchChannelLiveStreamNotificationHandler
    : INotificationHandler<TwitchWatchChannel>
{
    private readonly ILogger<TwitchWatchChannelLiveStreamNotificationHandler> _logger;
    private readonly ITwitchStreamStatus _twitchStreamStatus;

    public TwitchWatchChannelLiveStreamNotificationHandler(
        ITwitchStreamStatus twitchStreamStatus,
        ILogger<TwitchWatchChannelLiveStreamNotificationHandler> logger
    )
    {
        this._twitchStreamStatus =
            twitchStreamStatus ?? throw new ArgumentNullException(nameof(twitchStreamStatus));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ValueTask Handle(TwitchWatchChannel notification, CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer(notification);
        this._logger.EnablingForLiveStatusChecks(streamer);

        return this._twitchStreamStatus.EnableAsync(streamer);
    }

    private static Streamer Streamer(TwitchWatchChannel notification)
    {
        return notification.Info.UserName.ToStreamer();
    }
}
