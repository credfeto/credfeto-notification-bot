using Credfeto.Notification.Bot.Discord.BackgroundServices;
using Credfeto.Notification.Bot.Discord.Services;
using Credfeto.Notification.Bot.Shared;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Notification.Bot.Discord;

public static class DiscordSetup
{
    public static IServiceCollection AddDiscord(this IServiceCollection services)
    {
        return services.AddSingleton(_ => new DiscordSocketClient(new() { GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent }))
                       .AddSingleton<CommandService>()
                       .AddSingleton<IDiscordBot, DiscordBot>()
                       .AddSingleton<IRunOnStartup, DiscordCommandService>()
                       .AddSingleton<IDiscordConnectionService, DiscordConnectionService>()
                       .AddHostedService(x => x.GetRequiredService<IDiscordConnectionService>())
                       .AddHostedService<DiscordTestServices>();
    }
}