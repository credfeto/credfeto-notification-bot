using Credfeto.Notification.Bot.Discord.BackgroundServices;
using Credfeto.Notification.Bot.Discord.Services;
using Discord.WebSocket;
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
        return services.AddSingleton<DiscordSocketClient>()
                       .AddSingleton<IDiscordBot, DiscordBot>()
                       .AddSingleton<IDiscordConnectionService, DiscordConnectionService>()
                       .AddHostedService(x => x.GetRequiredService<IDiscordConnectionService>())
                       .AddHostedService<DiscordTestServices>();
    }
}