using System;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Actions.Services;
using Credfeto.Notification.Bot.Twitch.BackgroundServices;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Services;
using Credfeto.Notification.Bot.Twitch.Startup;
using Credfeto.Services.Startup.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using TwitchLib.Client;
using TwitchLib.Client.Interfaces;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Interfaces;
using TwitchLib.Communication.Models;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Interfaces;

namespace Credfeto.Notification.Bot.Twitch;

public static class TwitchSetup
{
    public static IServiceCollection AddTwitch(this IServiceCollection services)
    {
        return services.AddTwitchLib()
                       .AddServices()
                       .AddActions()
                       .AddHttpClients()
                       .AddStartupServices()
                       .AddBackgroundServices();
    }

    private static IServiceCollection AddHttpClients(this IServiceCollection services)
    {
        return ChannelFollowCount.RegisterHttpClient(services);
    }

    private static IServiceCollection AddTwitchLib(this IServiceCollection services)
    {
        return services.AddSingleton<ITwitchPubSub, TwitchPubSub>()
                       .AddSingleton<ITwitchClient, TwitchClient>()
                       .AddSingleton(_ => CreateWebSocketClient());
    }

    private static IClient CreateWebSocketClient()
    {
        return new WebSocketClient(new ClientOptions
                                   {
                                       MessagesAllowedInPeriod = 750,
                                       ThrottlingPeriod = TimeSpan.FromSeconds(30),
                                       ReconnectionPolicy = new(reconnectInterval: 1000, maxAttempts: null)
                                   });
    }

    private static IServiceCollection AddActions(this IServiceCollection services)
    {
        return services.AddSingleton<IRaidWelcome, RaidWelcome>()
                       .AddSingleton<ICustomTriggeredMessageSender, CustomTriggeredMessageSender>()
                       .AddSingleton<IShoutoutJoiner, ShoutoutJoiner>()
                       .AddSingleton<IContributionThanks, ContributionThanks>()
                       .AddSingleton<IWelcomeWaggon, WelcomeWaggon>();
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services.AddSingleton<ITwitchChannelManager, TwitchChannelManager>()
                       .AddSingleton<ITwitchChat, TwitchChat>()
                       .AddSingleton<ITwitchCustomMessageHandler, TwitchCustomMessageHandler>()
                       .AddSingleton<ITwitchMessageTriggerDebounceFilter, TwitchMessageTriggerDebounceFilter>()
                       .AddSingleton<ITwitchStreamStatus, TwitchStreamStatus>()
                       .AddSingleton<IUserInfoService, UserInfoService>()
                       .AddSingleton<ITwitchFollowerDetector, TwitchFollowerDetector>()
                       .AddSingleton<IChannelFollowCount, ChannelFollowCount>()
                       .AddSingleton<IFollowerMilestone, FollowerMilestone>()
                       .AddSingleton<ITwitchChatMessageGenerator, TwitchChatMessageGenerator>();
    }

    private static IServiceCollection AddBackgroundServices(this IServiceCollection services)
    {
        return services.AddHostedService<UpdateTwitchLiveStatusWorker>()
                       .AddHostedService<RestoreTwitchChatConnectionWorker>()
                       .AddHostedService<FollowersWorker>();
    }

    private static IServiceCollection AddStartupServices(this IServiceCollection services)
    {
        return services.AddRunOnStartupTask<TwitchChannelStartup>();
    }
}