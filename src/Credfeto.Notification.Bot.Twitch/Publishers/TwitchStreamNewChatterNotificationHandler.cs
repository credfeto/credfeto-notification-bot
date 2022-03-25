using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models.MediatorModels;
using MediatR;
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

    public async Task Handle(TwitchStreamNewChatter notification, CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Move the below into the handler.
            TwitchUser? twitchUser = await this._userInfoService.GetUserAsync(notification.User);

            if (twitchUser == null)
            {
                return;
            }

            this._logger.LogWarning($"Found {twitchUser.UserName}. Streamer: {twitchUser.IsStreamer} Created: {twitchUser.DateCreated}");

            this._logger.LogInformation($"{notification.Channel}: {twitchUser.UserName} - Regular: {notification.IsRegular}");

            if (notification.IsRegular)
            {
                this._logger.LogInformation($"{notification.Channel}: Hi @{twitchUser.UserName}");
                await this._welcomeWaggon.IssueWelcomeAsync(channel: notification.Channel, user: twitchUser.UserName, cancellationToken: cancellationToken);
            }

            if (twitchUser.IsStreamer)
            {
                await this._shoutoutJoiner.IssueShoutoutAsync(channel: notification.Channel, visitingStreamer: twitchUser, isRegular: notification.IsRegular, cancellationToken: cancellationToken);
            }
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Channel}: Failed to notify new chatter");
        }
    }
}