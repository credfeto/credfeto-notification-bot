using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Server.Helpers;
using Credfeto.Notification.Bot.Server.ServiceStartup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Server;

internal static class Program
{
    public static Task Main(string[] args)
    {
        StartupBanner.Show();

        return CreateHostBuilder(args).Build().InitializeLogging().RunAsync(CancellationToken.None);
    }

    private static IHost InitializeLogging(this IHost host)
    {
        ILoggerFactory loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();

        Logging.InitializeLogging(loggerFactory: loggerFactory);

        return host;
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseDefaultServiceProvider(InitialiseServiceProvider)
            .ConfigureServices(Service.Configure)
            .ConfigureLogging(InitialiseProviders)
            .UseWindowsService()
            .UseSystemd();
    }

    private static void InitialiseServiceProvider(ServiceProviderOptions options)
    {
        options.ValidateScopes = true;
        options.ValidateOnBuild = true;
    }

    private static void InitialiseProviders(HostBuilderContext hostBuilderContext, ILoggingBuilder logger)
    {
        logger.ClearProviders();
    }
}
