using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Discord.Services;

/// <summary>
///     Discord service.
/// </summary>
public sealed class DiscordConnectionService : DiscordLoggingBase, IDiscordConnectionService
{
    private readonly DiscordBotOptions _botConfiguration;
    private readonly DiscordSocketClient _discordSocketClient;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="options">Configuration settings.</param>
    /// <param name="discordSocketClient">Discord Socket client.</param>
    /// <param name="logger">Logging.</param>
    public DiscordConnectionService(IOptions<DiscordBotOptions> options, DiscordSocketClient discordSocketClient, ILogger<DiscordConnectionService> logger)
        : base(logger)
    {
        this._botConfiguration = options.Value ?? throw new ArgumentNullException(nameof(options));
        this._discordSocketClient = discordSocketClient ?? throw new ArgumentNullException(nameof(discordSocketClient));

        this._discordSocketClient.Log += this.LogAsync;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await this._discordSocketClient.LoginAsync(tokenType: TokenType.Bot, token: this._botConfiguration.Token);

        await this._discordSocketClient.StartAsync();

        await this._discordSocketClient.SetGameAsync(name: @"Twitch", streamUrl: null, type: ActivityType.Watching);
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        // and logout
        return this._discordSocketClient.LogoutAsync();
    }
}