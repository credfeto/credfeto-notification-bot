using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.BackgroundServices;

/// <summary>
///     Background service.
/// </summary>
public sealed class UpdateLiveStatusWorker : BackgroundService
{
    private readonly ILogger<UpdateLiveStatusWorker> _logger;
    private readonly ITwitchBot _twitchBot;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="twitchBot">Twitch Bot</param>
    /// <param name="logger">Logging.</param>
    /// <returns>Logging</returns>
    public UpdateLiveStatusWorker(ITwitchBot twitchBot, ILogger<UpdateLiveStatusWorker> logger)
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