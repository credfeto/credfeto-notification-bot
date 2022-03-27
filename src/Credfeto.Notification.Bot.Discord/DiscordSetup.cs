using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Notification.Bot.Discord;

/// <summary>
///     Configures Discord Integration
/// </summary>
public static class DiscordSetup
{
    /// <summary>
    ///     Configures discord integration.
    /// </summary>
    /// <param name="services">The Service collection to add the services to.</param>
    public static IServiceCollection AddDiscord(this IServiceCollection services)
    {
        return services.AddSingleton<DiscordSocketClient>();
    }
}