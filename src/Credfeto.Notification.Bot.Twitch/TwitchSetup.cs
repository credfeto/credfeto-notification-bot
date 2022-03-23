using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Actions.Services;
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
    public static IServiceCollection AddTwitch(this IServiceCollection services)
    {
        return services.AddServices()
                       .AddActions()
                       .AddHttpClients()
                       .AddBackgroundServices();
    }

    private static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        return ChannelFollowCount.RegisterHttpClient(services);
    }

    private static IServiceCollection AddActions(this IServiceCollection services)
    {
        return services.AddSingleton<IRaidWelcome, RaidWelcome>()
                       .AddSingleton<IHeistJoiner, HeistJoiner>()
                       .AddSingleton<IShoutoutJoiner, ShoutoutJoiner>()
                       .AddSingleton<IContributionThanks, ContributionThanks>()
                       .AddSingleton<IWelcomeWaggon, WelcomeWaggon>();
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddSingleton<ITwitchChannelManager, TwitchChannelManager>()
                       .AddSingleton<ITwitchChat, TwitchChat>()
                       .AddSingleton<ITwitchStreamStatus, TwitchStreamStatus>()
                       .AddSingleton<IUserInfoService, UserInfoService>()
                       .AddSingleton<IChannelFollowCount, ChannelFollowCount>()
                       .AddSingleton<IFollowerMilestone, FollowerMilestone>();
    }

    private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        return services.AddHostedService<UpdateTwitchLiveStatusWorker>()
                       .AddHostedService<RestoreTwitchChatConnectionWorker>();
    }
}