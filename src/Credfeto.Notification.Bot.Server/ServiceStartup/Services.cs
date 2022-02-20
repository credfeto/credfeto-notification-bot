using Credfeto.Notification.Bot.Server.Helpers;
using Credfeto.Notification.Bot.Server.Workers;
using Credfeto.Notification.Bot.Twitch;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Credfeto.Notification.Bot.Server.ServiceStartup;

internal static class Services
{
    public static void Configure(HostBuilderContext hostContext, IServiceCollection services)
    {
        services.AddOptions();

        hostContext.HostingEnvironment.ContentRootFileProvider = new NullFileProvider();

        Log.Logger = new LoggerConfiguration().Enrich.FromLogContext()
                                              .WriteTo.Console()
                                              .CreateLogger();

        IConfigurationRoot configurationRoot = LoadConfigFile();

        services.Configure<TwitchBotOptions>(configurationRoot.GetSection("Twitch"));
        TwitchSetup.Configure(services);

        services.AddHostedService<RetrieveStatusWorker>();
    }

    private static IConfigurationRoot LoadConfigFile()
    {
        return new ConfigurationBuilder().SetBasePath(ApplicationConfig.ConfigurationFilesPath)
                                         .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: false)
                                         .AddJsonFile(path: "appsettings-local.json", optional: true, reloadOnChange: false)
                                         .AddEnvironmentVariables()
                                         .Build();
    }
}