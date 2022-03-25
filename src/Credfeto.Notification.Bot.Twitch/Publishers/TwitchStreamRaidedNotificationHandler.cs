using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Models.MediatorModels;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

/// <summary>
///     Twitch stream raided notification handler.
/// </summary>
public sealed class TwitchStreamRaidedNotificationHandler : INotificationHandler<TwitchStreamRaided>
{
    private readonly ILogger<TwitchStreamRaidedNotificationHandler> _logger;
    private readonly IRaidWelcome _raidWelcome;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="raidWelcome">Raid welcome.</param>
    /// <param name="logger">Logging</param>
    public TwitchStreamRaidedNotificationHandler(IRaidWelcome raidWelcome, ILogger<TwitchStreamRaidedNotificationHandler> logger)
    {
        this._raidWelcome = raidWelcome;
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task Handle(TwitchStreamRaided notification, CancellationToken cancellationToken)
    {
        try
        {
            await this._raidWelcome.IssueRaidWelcomeAsync(channel: notification.Channel, raider: notification.Raider, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{notification.Channel}: Failed to notify stream raid");
        }
    }
}