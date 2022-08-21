using System;
using System.Text.Json.Serialization;
using Credfeto.Extensions.Configuration.Typed.Json;
using Credfeto.Notification.Bot.Database;
using Credfeto.Notification.Bot.Database.Pgsql;
using Credfeto.Notification.Bot.Database.Shared;
using Credfeto.Notification.Bot.Discord;
using Credfeto.Notification.Bot.Server.Helpers;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Configuration.Validators;
using FluentValidation.Results;
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
        IConfigurationRoot configurationRoot = LoadConfigFile();

        JsonSerializerContext jsonSerializerContext = ServerConfigurationSerializationContext.Default;

        try
        {
            return services.AddOptions()
                           .WithConfiguration<PgsqlServerConfigurationValidator, PgsqlServerConfiguration>(configurationRoot: configurationRoot,
                                                                                                           key: "Database:Postgres",
                                                                                                           jsonSerializerContext: jsonSerializerContext)
                           .WithConfiguration<DiscordBotOptionsValidator, DiscordBotOptions>(configurationRoot: configurationRoot,
                                                                                             key: "Discord",
                                                                                             jsonSerializerContext: jsonSerializerContext)
                           .WithConfiguration<TwitchBotOptionsValidator, TwitchBotOptions>(configurationRoot: configurationRoot,
                                                                                           key: "Twitch",
                                                                                           jsonSerializerContext: jsonSerializerContext);
        }
        catch (ConfigurationErrorsException exception)
        {
            Console.WriteLine("Configuration errors: ");

            foreach (ValidationFailure error in exception.Errors)
            {
                Console.WriteLine($" * {error.PropertyName}: {error.ErrorMessage}");
            }

            throw;
        }
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