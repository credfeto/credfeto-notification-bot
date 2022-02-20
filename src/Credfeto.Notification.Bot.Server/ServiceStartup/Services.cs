using Credfeto.Notification.Bot.Server.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Credfeto.Notification.Bot.Server.ServiceStartup;

internal static class Services
{
    public static void Configure(HostBuilderContext hostContext, IServiceCollection services)
    {
        services.AddHostedService<RetrieveStatusWorker>();
    }
}