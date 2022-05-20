using Credfeto.Notification.Bot.Database.Interfaces.Builders;
using Credfeto.Notification.Bot.Database.Shared.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Notification.Bot.Database.Shared;

public static class SharedDatabaseSetup
{
    public static IServiceCollection AddDatabaseShared(this IServiceCollection services)
    {
        return services.AddSingleton(typeof(IObjectCollectionBuilder<,>), typeof(ObjectCollectionBuilder<,>));
    }
}