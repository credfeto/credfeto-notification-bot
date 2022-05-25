using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Discord.BackgroundServices;

public sealed class DiscordTestServices : BackgroundService
{
    private readonly IDiscordBot _discordBot;
    private readonly ILogger<DiscordTestServices> _logger;

    public DiscordTestServices(IDiscordBot discordBot, ILogger<DiscordTestServices> logger)
    {
        this._discordBot = discordBot;
        this._logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this._logger.LogDebug($"Starting {this._discordBot.GetType().FullName}");

        return Task.CompletedTask;
    }
}