using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchWatchChannelLiveStreamNotificationHandler : INotificationHandler<TwitchWatchChannel>
{
    private readonly ILogger<TwitchWatchChannelLiveStreamNotificationHandler> _logger;
    private readonly ITwitchStreamStatus _twitchStreamStatus;

    public TwitchWatchChannelLiveStreamNotificationHandler(ITwitchStreamStatus twitchStreamStatus, ILogger<TwitchWatchChannelLiveStreamNotificationHandler> logger)
    {
        this._twitchStreamStatus = twitchStreamStatus ?? throw new ArgumentNullException(nameof(twitchStreamStatus));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(TwitchWatchChannel notification, CancellationToken cancellationToken)
    {
        Streamer streamer = notification.Info.UserName.ToStreamer();
        this._logger.LogInformation($"{streamer}: Enabling for live status checks");
        this._twitchStreamStatus.Enable(streamer);

        return Task.CompletedTask;
    }
}