using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared.Services.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Shared.Services;

public sealed class StartupService : BackgroundService
{
    private readonly ILogger<StartupService> _logger;
    private readonly IEnumerable<IRunOnStartup> _services;

    public StartupService(IEnumerable<IRunOnStartup> services, ILogger<StartupService> logger)
    {
        this._services = services ?? throw new ArgumentNullException(nameof(services));
        this._logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.WhenAll(this._services.Select(service => this.StartServiceAsync(service: service, cancellationToken: stoppingToken)));
    }

    private Task StartServiceAsync(IRunOnStartup service, in CancellationToken cancellationToken)
    {
        this._logger.LogStarting(service.GetType()
                                        .Name);

        return service.StartAsync(cancellationToken);
    }
}