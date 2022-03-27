using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Credfeto.Notification.Bot.Discord.BackgroundServices;

public sealed class DiscordTestServices : BackgroundService
{
    private readonly IDiscordBot _discordBot;

    public DiscordTestServices(IDiscordBot discordBot)
    {
        this._discordBot = discordBot;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}