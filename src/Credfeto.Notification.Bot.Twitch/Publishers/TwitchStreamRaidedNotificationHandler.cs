using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchStreamRaidedNotificationHandler : INotificationHandler<TwitchStreamRaided>
{
    private readonly ILogger<TwitchStreamRaidedNotificationHandler> _logger;
    private readonly IRaidWelcome _raidWelcome;

    public TwitchStreamRaidedNotificationHandler(IRaidWelcome raidWelcome, ILogger<TwitchStreamRaidedNotificationHandler> logger)
    {
        this._raidWelcome = raidWelcome;
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(TwitchStreamRaided notification, CancellationToken cancellationToken)
    {
        try
        {
            await this._raidWelcome.IssueRaidWelcomeAsync(streamer: notification.Streamer, raider: notification.Raider, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Streamer}: Failed to notify stream raid");
        }
    }
}