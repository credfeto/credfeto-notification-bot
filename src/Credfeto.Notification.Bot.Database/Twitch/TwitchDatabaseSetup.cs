using Credfeto.Notification.Bot.Database.Twitch.DataManagers;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Notification.Bot.Database.Twitch;

internal static class TwitchDatabaseSetup
{
    public static IServiceCollection AddTwitch(this IServiceCollection services)
    {
        return services.AddDataManagers();
    }

    private static IServiceCollection AddDataManagers(this IServiceCollection services)
    {
        return services.AddSingleton<ITwitchStreamDataManager, TwitchStreamDataManager>()
                       .AddSingleton<ITwitchViewerDataManager, TwitchViewerDataManager>()
                       .AddSingleton<ITwitchStreamerDataManager, TwitchStreamerDataManager>();
    }
}