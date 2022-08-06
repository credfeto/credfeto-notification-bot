using System.IO;
using Credfeto.Notification.Bot.Database.Pgsql;
using Credfeto.Notification.Bot.Shared;
using Credfeto.NotificationBot.Shared.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Notification.Bot.Database.Tests.Integration.Setup;

internal static class IntegrationTestStartup
{
    private static readonly object InitLock = new();

    public static IServiceCollection ConfigureServices(IServiceCollection services)
    {
        lock (InitLock)
        {
            IConfigurationRoot configurationRoot = BuildConfiguration();

            return services.AddSingleton(configurationRoot)
                           .AddOptions()
                           .WithConfiguration(configurationRoot: configurationRoot,
                                              key: "Database:Postgres",
                                              validator: new PgsqlServerConfigurationValidator(),
                                              jsonSerializerContext: DatabaseConfigurationSerializationContext.Default)
                           .AddPostgresql()
                           .AddApplicationDatabase()
                           .AddResources();
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