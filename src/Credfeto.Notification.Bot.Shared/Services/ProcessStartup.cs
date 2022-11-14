using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared.Services.Logging;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Shared.Services;

public sealed class ProcessStartup : IRunOnStartup
{
    private readonly ILogger<ProcessStartup> _logger;

    public ProcessStartup(ILogger<ProcessStartup> logger)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        this._logger.LogStarting();

        return Task.CompletedTask;
    }
}