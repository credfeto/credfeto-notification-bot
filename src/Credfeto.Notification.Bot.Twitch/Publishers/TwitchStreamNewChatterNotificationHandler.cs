using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchStreamNewChatterNotificationHandler : INotificationHandler<TwitchStreamNewChatter>
{
    private readonly ILogger<TwitchStreamNewChatterNotificationHandler> _logger;
    private readonly IShoutoutJoiner _shoutoutJoiner;
    private readonly IUserInfoService _userInfoService;
    private readonly IWelcomeWaggon _welcomeWaggon;

    public TwitchStreamNewChatterNotificationHandler(IUserInfoService userInfoService,
                                                     IWelcomeWaggon welcomeWaggon,
                                                     IShoutoutJoiner shoutoutJoiner,
                                                     ILogger<TwitchStreamNewChatterNotificationHandler> logger)
    {
        this._userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        this._welcomeWaggon = welcomeWaggon ?? throw new ArgumentNullException(nameof(welcomeWaggon));
        this._shoutoutJoiner = shoutoutJoiner ?? throw new ArgumentNullException(nameof(shoutoutJoiner));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask Handle(TwitchStreamNewChatter notification, CancellationToken cancellationToken)
    {
        try
        {
            TwitchUser? twitchUser = await this._userInfoService.GetUserAsync(userName: notification.User, cancellationToken: cancellationToken);

            if (twitchUser == null)
            {
                return;
            }

            this._logger.LogWarning($"Found {twitchUser.UserName}. Streamer: {twitchUser.IsStreamer} Created: {twitchUser.DateCreated}");

            this._logger.LogInformation($"{notification.Streamer}: {twitchUser.UserName} - Regular: {notification.IsRegular}");

            if (notification.IsRegular)
            {
                await this._welcomeWaggon.IssueWelcomeAsync(streamer: notification.Streamer, user: twitchUser.UserName, cancellationToken: cancellationToken);
            }

            if (twitchUser.IsStreamer)
            {
                if (notification.Streamer != twitchUser.UserName.ToStreamer())
                {
                    // Only shoutout streamers, IF they're not the streamer themselves
                    await this._shoutoutJoiner.IssueShoutoutAsync(streamer: notification.Streamer,
                                                                  visitingStreamer: twitchUser,
                                                                  isRegular: notification.IsRegular,
                                                                  cancellationToken: cancellationToken);
                }
            }
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Streamer}: Failed to notify new chatter");
        }
    }
}