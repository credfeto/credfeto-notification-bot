using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchWatchChannelFollowerNotificationHandler : INotificationHandler<TwitchWatchChannel>
{
    private readonly ITwitchFollowerDetector _followerDetector;
    private readonly ILogger<TwitchWatchChannelFollowerNotificationHandler> _logger;

    public TwitchWatchChannelFollowerNotificationHandler(ITwitchFollowerDetector followerDetector, ILogger<TwitchWatchChannelFollowerNotificationHandler> logger)
    {
        this._followerDetector = followerDetector ?? throw new ArgumentNullException(nameof(followerDetector));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(TwitchWatchChannel notification, CancellationToken cancellationToken)
    {
        Streamer streamer = notification.Info.UserName.ToStreamer();
        this._logger.LogInformation($"{streamer}: Enabling for follower tracking");
        this._followerDetector.Enable(notification.Info);

        return Task.CompletedTask;
    }
}