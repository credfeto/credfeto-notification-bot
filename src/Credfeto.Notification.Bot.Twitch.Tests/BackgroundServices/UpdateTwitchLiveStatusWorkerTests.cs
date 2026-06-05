using System;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.BackgroundServices;
using FunFair.Test.Common;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.BackgroundServices;

public sealed class UpdateTwitchLiveStatusWorkerTests : LoggingTestBase
{
    private readonly ITwitchStreamStatus _twitchStreamStatus;
    private readonly IHostedService _worker;

    public UpdateTwitchLiveStatusWorkerTests(ITestOutputHelper output)
        : base(output)
    {
        this._twitchStreamStatus = GetSubstitute<ITwitchStreamStatus>();

        this._worker = new UpdateTwitchLiveStatusWorker(
            twitchStreamStatus: this._twitchStreamStatus,
            logger: this.GetTypedLogger<UpdateTwitchLiveStatusWorker>()
        );
    }

    [Fact]
    public async Task ExecuteShouldCallUpdateAsync()
    {
        using System.Threading.CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(200));

        this._twitchStreamStatus.UpdateAsync().Returns(Task.CompletedTask);

        await this._worker.StartAsync(cts.Token);
        await Task.Delay(delay: TimeSpan.FromMilliseconds(300), cancellationToken: this.CancellationToken());
        await this._worker.StopAsync(this.CancellationToken());

        await this._twitchStreamStatus.Received(1).UpdateAsync();
    }

    [Fact]
    public async Task ExceptionInUpdateAsyncShouldBeSwallowed()
    {
        using System.Threading.CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(200));

        this._twitchStreamStatus.UpdateAsync()
            .Returns(Task.FromException(new InvalidOperationException("Test exception")));

        await this._worker.StartAsync(cts.Token);
        await Task.Delay(delay: TimeSpan.FromMilliseconds(300), cancellationToken: this.CancellationToken());
        await this._worker.StopAsync(this.CancellationToken());

        await this._twitchStreamStatus.Received(1).UpdateAsync();
    }
}
