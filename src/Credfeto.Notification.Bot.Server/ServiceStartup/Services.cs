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

        IConfigurationRoot configurationRoot = LoadConfigFile();

        services.AddOptions()
                .AddMediatR(typeof(Program), typeof(DiscordSetup), typeof(TwitchSetup))
                .AddAppLogging()
                .AddResources()
                .Configure<PgsqlServerConfiguration>(configurationRoot.GetSection("Database:Postgres"))
                .AddPostgresql()
                .AddDatabaseShared()
                .AddApplicationDatabase()
                .Configure<DiscordBotOptions>(configurationRoot.GetSection("Discord"))
                .AddDiscord()
                .Configure<TwitchBotOptions>(configurationRoot.GetSection("Twitch"))
                .AddTwitch();
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