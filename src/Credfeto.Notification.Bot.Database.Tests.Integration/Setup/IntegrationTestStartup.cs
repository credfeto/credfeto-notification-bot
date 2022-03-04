using System.IO;
using Credfeto.Notification.Bot.Database.Pgsql;
using Credfeto.Notification.Bot.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Notification.Bot.Database.Tests.Integration.Setup;

internal static class IntegrationTestStartup
{
    private static readonly object InitLock = new();

    public static void ConfigureServices(IServiceCollection services)
    {
        IConfigurationRoot configurationRoot = BuildConfiguration();

        lock (InitLock)
        {
            services.AddSingleton(configurationRoot)
                    .AddOptions()
                    .Configure<PgsqlServerConfiguration>(configurationRoot.GetSection("Database:Postgresql"))
                    .ConfigurePostgresql()
                    .ConfigureDatabase()
                    .ConfigureResources();
        }
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        string settingsPath = Directory.GetCurrentDirectory();

        IConfigurationBuilder integrationConfigurationBuilder = new ConfigurationBuilder().SetBasePath(settingsPath)
                                                                                          .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: false)
                                                                                          .AddJsonFile(path: "appsettings-local.json", optional: true, reloadOnChange: false)
                                                                                          .AddEnvironmentVariables();

        return integrationConfigurationBuilder.Build();
    }
}