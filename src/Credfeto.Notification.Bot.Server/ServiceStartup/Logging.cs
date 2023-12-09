using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;

namespace Credfeto.Notification.Bot.Server.ServiceStartup;

internal static class Logging
{
    public static IServiceCollection AddAppLogging(this IServiceCollection services)
    {
        // add logging to the services
        return services.AddLogging(AddFilters);
    }

    [SuppressMessage(category: "Microsoft.Reliability", checkId: "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Lives for program lifetime")]
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Not easily testable as uses third party services")]
    [SuppressMessage(category: "SmartAnalyzers.CSharpExtensions.Annotations", checkId: "CSE007:DisposeObjectsBeforeLosingScope", Justification = "Lives for program lifetime")]
    public static void InitializeLogging(ILoggerFactory loggerFactory)
    {
        // set up Serilog logger
        Logger logger = CreateLogger();

        // set up the logger factory
        loggerFactory.AddSerilog(logger);
    }

    private static Logger CreateLogger()
    {
        string processName = typeof(Program).Namespace ?? "Credfeto.Notification.Bot.Server";

        return new LoggerConfiguration().Enrich.FromLogContext()
                                        .Enrich.WithDemystifiedStackTraces()
                                        .Enrich.WithMachineName()
                                        .Enrich.WithProcessId()
                                        .Enrich.WithThreadId()
                                        .Enrich.WithProperty(name: "ProcessName", value: processName)
                                        .WriteTo.Console()
                                        .CreateLogger();
    }

    private static void AddFilters(ILoggingBuilder builder)
    {
        builder.AddFilter(category: "Discord", level: LogLevel.Warning)
               .AddFilter(category: "Microsoft", level: LogLevel.Warning)
               .AddFilter(category: "System.Net.Http.HttpClient", level: LogLevel.Warning)
               .AddFilter(category: "Microsoft.AspNetCore.ResponseCaching.ResponseCachingMiddleware", level: LogLevel.Error)
               .AddFilter(category: "TwitchLib", level: LogLevel.Warning);
    }
}