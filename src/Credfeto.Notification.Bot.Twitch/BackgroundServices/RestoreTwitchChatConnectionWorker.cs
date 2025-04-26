using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.BackgroundServices.LoggingExgtensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.BackgroundServices;

public sealed class RestoreTwitchChatConnectionWorker : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(30);

    private readonly ILogger<RestoreTwitchChatConnectionWorker> _logger;
    private readonly ITwitchChat _twitchChat;

    public RestoreTwitchChatConnectionWorker(ITwitchChat twitchChat, ILogger<RestoreTwitchChatConnectionWorker> logger)
    {
        this._twitchChat = twitchChat ?? throw new ArgumentNullException(nameof(twitchChat));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

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
        catch (Exception exception)
        {
            this._logger.FailedToUpdateTwitchChatConnection(message: exception.Message, exception: exception);

            throw;
        }
    }
}
