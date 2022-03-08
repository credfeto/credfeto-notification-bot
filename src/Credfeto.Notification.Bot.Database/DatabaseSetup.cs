using Credfeto.Notification.Bot.Database.Twitch;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Notification.Bot.Database;

public static class DatabaseSetup
{
    /// <summary>
    ///     Configures database.
    /// </summary>
    /// <param name="services">The DI Container to register services in.</param>
    public static IServiceCollection AddApplicationDatabase(this IServiceCollection services)
    {
        return services.AddTwitch();
    }
}