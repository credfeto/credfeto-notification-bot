using System;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.BackgroundServices;
using FunFair.Test.Common;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.BackgroundServices;

public sealed class RestoreTwitchChatConnectionWorkerTests : LoggingTestBase
{
    private readonly ITwitchChat _twitchChat;
    private readonly IHostedService _worker;

    public RestoreTwitchChatConnectionWorkerTests(ITestOutputHelper output)
        : base(output)
    {
        this._twitchChat = GetSubstitute<ITwitchChat>();

        this._worker = new RestoreTwitchChatConnectionWorker(
            twitchChat: this._twitchChat,
            logger: this.GetTypedLogger<RestoreTwitchChatConnectionWorker>()
        );
    }

    [Fact]
    public async Task ExecuteShouldCallUpdateAsync()
    {
        using System.Threading.CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(200));

        this._twitchChat.UpdateAsync().Returns(Task.CompletedTask);

        await this._worker.StartAsync(cts.Token);
        await Task.Delay(delay: TimeSpan.FromMilliseconds(300), cancellationToken: this.CancellationToken());
        await this._worker.StopAsync(this.CancellationToken());

        await this._twitchChat.Received(1).UpdateAsync();
    }

    [Fact]
    public async Task ExceptionInUpdateAsyncShouldPropagate()
    {
        using System.Threading.CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(200));

        this._twitchChat.UpdateAsync().Returns(Task.FromException(new InvalidOperationException("Test exception")));

        await this._worker.StartAsync(cts.Token);
        await Task.Delay(delay: TimeSpan.FromMilliseconds(300), cancellationToken: this.CancellationToken());
        await this._worker.StopAsync(this.CancellationToken());

        await this._twitchChat.Received(1).UpdateAsync();
    }
}
