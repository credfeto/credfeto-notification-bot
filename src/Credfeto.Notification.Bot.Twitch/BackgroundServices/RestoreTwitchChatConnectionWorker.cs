using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.BackgroundServices;

/// <summary>
///     Background service for connecting twitch status.
/// </summary>
public sealed class RestoreTwitchChatConnectionWorker : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

    private readonly ILogger<RestoreTwitchChatConnectionWorker> _logger;
    private readonly ITwitchChat _twitchChat;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="twitchChat">Twitch Chat</param>
    /// <param name="logger">Logging.</param>
    /// <returns>Logging</returns>
    public RestoreTwitchChatConnectionWorker(ITwitchChat twitchChat, ILogger<RestoreTwitchChatConnectionWorker> logger)
    {
        this._twitchChat = twitchChat ?? throw new ArgumentNullException(nameof(twitchChat));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await this.UpdateStatusAsync();
            await Task.Delay(delay: Interval, cancellationToken: stoppingToken);
        }
    }

    private async Task UpdateStatusAsync()
    {
        try
        {
            await this._twitchChat.UpdateAsync();
        }
        catch (Exception e)
        {
            this._logger.LogError(new(e.HResult), exception: e, message: "Failed to update twitch chat connection");

            throw;
        }
    }
}