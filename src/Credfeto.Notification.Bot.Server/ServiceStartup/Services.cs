using Credfeto.Notification.Bot.Database;
using Credfeto.Notification.Bot.Database.Pgsql;
using Credfeto.Notification.Bot.Database.Shared;
using Credfeto.Notification.Bot.Discord;
using Credfeto.Notification.Bot.Server.Helpers;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch;
using Credfeto.Notification.Bot.Twitch.Configuration;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;

namespace Credfeto.Notification.Bot.Server.ServiceStartup;

internal static class Services
{
    public static void Configure(HostBuilderContext hostContext, IServiceCollection services)
    {
        hostContext.HostingEnvironment.ContentRootFileProvider = new NullFileProvider();

        Log.Logger = CreateLogger();

        services.AddConfiguration()
                .AddMediatR(typeof(Program), typeof(DiscordSetup), typeof(TwitchSetup))
                .AddAppLogging()
                .AddResources()
                .AddPostgresql()
                .AddDatabaseShared()
                .AddApplicationDatabase()
                .AddDiscord()
                .AddTwitch();
    }

    private static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        //Microsoft.Extensions.Configuration.Binder.
        IConfigurationRoot configurationRoot = LoadConfigFile();

        return services.AddOptions()
                       .Configure<PgsqlServerConfiguration>(configurationRoot.GetSection("Database:Postgres"))
                       .Configure<DiscordBotOptions>(configurationRoot.GetSection("Discord"))
                       .Configure<TwitchBotOptions>(configurationRoot.GetSection("Twitch"));
    }

    private static Logger CreateLogger()
    {
        return new LoggerConfiguration().Enrich.FromLogContext()
                                        .WriteTo.Console()
                                        .CreateLogger();
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