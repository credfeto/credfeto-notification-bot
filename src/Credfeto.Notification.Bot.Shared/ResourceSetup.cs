using Credfeto.Notification.Bot.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Notification.Bot.Shared;

/// <summary>
///     Configures Resources
/// </summary>
public static class ResourceSetup
{
    /// <summary>
    ///     Configures resources.
    /// </summary>
    /// <param name="services"></param>
    public static IServiceCollection ConfigureResources(this IServiceCollection services)
    {
        return AddResources(services);
    }

    private static IServiceCollection AddResources(this IServiceCollection services)
    {
        return services.AddSingleton<ICurrentTimeSource, CurrentTimeSource>()
                       .AddSingleton(typeof(IMessageChannel<>), typeof(MessageChannel<>));
    }
}