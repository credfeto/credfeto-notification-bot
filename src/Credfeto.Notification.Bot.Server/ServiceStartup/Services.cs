using Credfeto.Notification.Bot.Server.Workers;
using Credfeto.Notification.Bot.Twitch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Credfeto.Notification.Bot.Server.ServiceStartup;

internal static class Services
{
    public static void Configure(HostBuilderContext hostContext, IServiceCollection services)
    {
        TwitchSetup.Configure(services);

        services.AddHostedService<RetrieveStatusWorker>();
    }
}