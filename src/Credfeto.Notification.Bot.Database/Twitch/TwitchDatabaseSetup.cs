using Credfeto.Database.Interfaces.Builders;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Entities;
using Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;
using Credfeto.Notification.Bot.Database.Twitch.DataManagers;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Notification.Bot.Database.Twitch;

internal static class TwitchDatabaseSetup
{
    public static IServiceCollection AddTwitch(this IServiceCollection services)
    {
        return services.AddObjectBuilders()
                       .AddDataManagers();
    }

    private static IServiceCollection AddObjectBuilders(this IServiceCollection services)
    {
        return services.AddSingleton<IObjectBuilder<TwitchViewerUserEntity, TwitchUser>, TwitchViewerUserBuilder>()
                       .AddSingleton<IObjectBuilder<TwitchStreamerUserEntity, TwitchUser>, TwitchStreamerUserBuilder>()
                       .AddSingleton<IObjectBuilder<TwitchChatterEntity, TwitchChatter>, TwitchChatterBuilder>()
                       .AddSingleton<IObjectBuilder<TwitchRegularChatterEntity, TwitchRegularChatter>, TwitchRegularChatterBuilder>()
                       .AddSingleton<IObjectBuilder<TwitchFollowerMilestoneEntity, TwitchFollowerMilestone>, TwitchFollowerMilestoneBuilder>()
                       .AddSingleton<IObjectBuilder<TwitchFollowerEntity, TwitchFollower>, TwitchFollowerBuilder>()
                       .AddSingleton<IObjectBuilder<StreamSettingsEntity, StreamSettings>, TwitchStreamSettingsBuilder>();
    }

    private static IServiceCollection AddDataManagers(this IServiceCollection services)
    {
        return services.AddSingleton<ITwitchStreamDataManager, TwitchStreamDataManager>()
                       .AddSingleton<ITwitchViewerDataManager, TwitchViewerDataManager>()
                       .AddSingleton<ITwitchStreamerDataManager, TwitchStreamerDataManager>();
    }
}