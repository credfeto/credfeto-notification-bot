using Credfeto.Notification.Bot.Twitch.BackgroundServices;
using Credfeto.Notification.Bot.Twitch.Resources;
using Credfeto.Notification.Bot.Twitch.Resources.Services;
using Credfeto.Notification.Bot.Twitch.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Notification.Bot.Twitch;

/// <summary>
///     Configures Twitch Integration
/// </summary>
public static class TwitchSetup
{
    /// <summary>
    ///     Configures twitch integration.
    /// </summary>
    /// <param name="services"></param>
    public static IServiceCollection Configure(IServiceCollection services)
    {
        return AddResources(services)
               .AddServices()
               .AddBackgroundServices();
    }

    private static IServiceCollection AddResources(this IServiceCollection services)
    {
        return services.AddSingleton<ICurrentTimeSource, CurrentTimeSource>()
                       .AddSingleton(typeof(IMessageChannel<>), typeof(MessageChannel<>));
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddSingleton<ITwitchChannelManager, TwitchChannelManager>()
                       .AddSingleton<ITwitchChat, TwitchChat>()
                       .AddSingleton<ITwitchStreamStatus, TwitchStreamStatus>()
                       .AddSingleton<IRaidWelcome, RaidWelcome>();
    }

    private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        return services.AddHostedService<UpdateLiveStatusWorker>();
    }
}