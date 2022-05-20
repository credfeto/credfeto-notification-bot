using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchWatchChannelFollowerNotificationHandler : INotificationHandler<TwitchWatchChannel>
{
    private readonly ITwitchFollowerDetector _followerDetector;
    private readonly ILogger<TwitchWatchChannelFollowerNotificationHandler> _logger;
    private readonly TwitchBotOptions _options;

    public TwitchWatchChannelFollowerNotificationHandler(IOptions<TwitchBotOptions> options,
                                                         ITwitchFollowerDetector followerDetector,
                                                         ILogger<TwitchWatchChannelFollowerNotificationHandler> logger)
    {
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        this._followerDetector = followerDetector ?? throw new ArgumentNullException(nameof(followerDetector));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Handle(TwitchWatchChannel notification, CancellationToken cancellationToken)
    {
        Streamer streamer = notification.Info.UserName.ToStreamer();

        if (!this._options.IsModChannel(streamer))
        {
            // don't watch for followers for channels we're not a mod for
            return Task.CompletedTask;
        }

        this._logger.LogInformation($"{streamer}: Enabling for follower tracking");

        return this._followerDetector.EnableAsync(notification.Info);
    }
}