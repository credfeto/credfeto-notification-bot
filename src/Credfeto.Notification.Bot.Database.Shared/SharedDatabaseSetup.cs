using Credfeto.Notification.Bot.Database.Interfaces.Builders;
using Credfeto.Notification.Bot.Database.Shared.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Notification.Bot.Database.Shared;

/// <summary>
///     Shared database infrastructure setup.
/// </summary>
public static class SharedDatabaseSetup
{
    /// <summary>
    ///     Configures database.
    /// </summary>
    /// <param name="services">The DI Container to register services in.</param>
    public static IServiceCollection ConfigureDatabaseShared(this IServiceCollection services)
    {
        return services.AddSingleton(typeof(IObjectCollectionBuilder<,>), typeof(ObjectCollectionBuilder<,>));
    }
}