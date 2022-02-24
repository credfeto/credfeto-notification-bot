using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Server.Workers;

/// <summary>
///     Background service.
/// </summary>
public sealed class RetrieveStatusWorker : BackgroundService
{
    private readonly ILogger<RetrieveStatusWorker> _logger;
    private readonly ITwitchBot _twitchBot;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="twitchBot">Twitch Bot</param>
    /// <param name="logger">Logging.</param>
    /// <returns>Logging</returns>
    public RetrieveStatusWorker(ITwitchBot twitchBot, ILogger<RetrieveStatusWorker> logger)
    {
        this._twitchBot = twitchBot ?? throw new ArgumentNullException(nameof(twitchBot));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await this.UpdateStatusAsync();
            await Task.Delay(TimeSpan.FromSeconds(value: 30), cancellationToken: stoppingToken);
        }
    }

    private async Task UpdateStatusAsync()
    {
        try
        {
            await this._twitchBot.UpdateAsync();
        }
        catch (Exception e)
        {
            this._logger.LogError(new(e.HResult), exception: e, message: "Failed to update twitch status");

            throw;
        }
    }
}