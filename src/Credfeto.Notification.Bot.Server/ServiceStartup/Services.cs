﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Credfeto.Notification.Bot.Server.Helpers;
using Credfeto.Notification.Bot.Server.Workers;
using Credfeto.Notification.Bot.Twitch;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

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

internal static class Logging
{
    /// <summary>
    ///     Configures logging in dependency injection.
    /// </summary>
    /// <param name="services">The dependency injection collection to add the services to</param>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Not easily testable as uses third party services")]
    public static void Configure(IServiceCollection services)
    {
        // add logging to the services
        services.AddLogging(AddFilters);
    }

    /// <summary>
    ///     Initialise logging.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    [SuppressMessage(category: "Microsoft.Reliability", checkId: "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Lives for program lifetime")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Not easily testable as uses third party services")]
    public static void InitializeLogging(ILoggerFactory loggerFactory)
    {
        // set up Serilog logger
        Log.Logger = CreateLogger();

        // set up the logger factory
        loggerFactory.AddSerilog();
    }

    private static Logger CreateLogger()
    {
        return new LoggerConfiguration().Enrich.FromLogContext()
                                        .Enrich.WithMachineName()
                                        .Enrich.WithProcessId()
                                        .Enrich.WithThreadId()
                                        .Enrich.WithProperty(name: @"ProcessName", typeof(Program).Namespace!)
                                        .WriteToDebuggerAwareOutput()
                                        .CreateLogger();
    }

    private static void AddFilters(ILoggingBuilder builder)
    {
        builder.AddFilter(category: @"Microsoft", level: LogLevel.Warning)
               .AddFilter(category: @"System.Net.Http.HttpClient", level: LogLevel.Warning)
               .AddFilter(category: @"Microsoft.AspNetCore.ResponseCaching.ResponseCachingMiddleware", level: LogLevel.Error);
    }

    private static LoggerConfiguration WriteToDebuggerAwareOutput(this LoggerConfiguration configuration)
    {
        if (Debugger.IsAttached)
        {
            configuration = configuration.WriteTo.Debug();
        }
        else
        {
            configuration = configuration.WriteTo.Console();
        }

        return configuration;
    }
}