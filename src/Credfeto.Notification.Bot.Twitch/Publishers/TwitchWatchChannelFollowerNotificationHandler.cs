using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.Models;
using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchWatchChannelFollowerNotificationHandler : INotificationHandler<TwitchWatchChannel>
{
    private readonly ITwitchFollowerDetector _followerDetector;
    private readonly ILogger<TwitchWatchChannelFollowerNotificationHandler> _logger;
    private readonly TwitchBotOptions _options;

    public TwitchWatchChannelFollowerNotificationHandler(IOptions<TwitchBotOptions> options, ITwitchFollowerDetector followerDetector, ILogger<TwitchWatchChannelFollowerNotificationHandler> logger)
    {
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        this._followerDetector = followerDetector ?? throw new ArgumentNullException(nameof(followerDetector));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ValueTask Handle(TwitchWatchChannel notification, CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer(notification);

        if (!this._options.IsModChannel(streamer))
        {
            // don't watch for followers for channels we're not a mod for
            return ValueTask.CompletedTask;
        }

        this._logger.LogInformation($"{streamer}: Enabling for follower tracking");

        return this._followerDetector.EnableAsync(streamer: notification.Info, cancellationToken: cancellationToken);
    }

    private static Streamer Streamer(TwitchWatchChannel notification)
    {
        return notification.Info.UserName.ToStreamer();
    }
}