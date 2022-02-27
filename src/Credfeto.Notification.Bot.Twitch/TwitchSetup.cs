using Credfeto.Notification.Bot.Twitch.BackgroundServices;
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
    public static IServiceCollection ConfigureTwitch(this IServiceCollection services)
    {
        return services.AddServices()
                       .AddBackgroundServices();
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddSingleton<ITwitchChannelManager, TwitchChannelManager>()
                       .AddSingleton<ITwitchChat, TwitchChat>()
                       .AddSingleton<ITwitchStreamStatus, TwitchStreamStatus>()
                       .AddSingleton<IRaidWelcome, RaidWelcome>()
                       .AddSingleton<IHeistJoiner, HeistJoiner>()
                       .AddSingleton<IShoutoutJoiner, ShoutoutJoiner>();
    }

    private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        return services.AddHostedService<UpdateTwitchLiveStatusWorker>()
                       .AddHostedService<RestoreTwitchChatConnectionWorker>();
    }
}