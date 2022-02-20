using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Server.Workers;

/// <summary>
/// Background service.
/// </summary>
public sealed class RetrieveStatusWorker : BackgroundService
{
    private readonly ILogger<RetrieveStatusWorker> _logger;

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public RetrieveStatusWorker(ILogger<RetrieveStatusWorker> logger)
    {
        this._logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await this.UpdateStatusAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(value: 30), cancellationToken: stoppingToken);
        }
    }

    private Task UpdateStatusAsync(CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Tick");

        return Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken: cancellationToken);
    }
}