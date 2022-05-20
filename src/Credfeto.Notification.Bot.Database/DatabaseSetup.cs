using Credfeto.Notification.Bot.Database.Twitch;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Notification.Bot.Database;

public static class DatabaseSetup
{
    public static IServiceCollection AddApplicationDatabase(this IServiceCollection services)
    {
        return services.AddTwitch();
    }
}