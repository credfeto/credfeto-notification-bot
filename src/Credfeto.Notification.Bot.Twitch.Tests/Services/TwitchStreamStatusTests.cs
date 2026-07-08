using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using Mediator;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Services;

public sealed class TwitchStreamStatusTests : LoggingTestBase
{
    private readonly ILiveStreamMonitor _liveStreamMonitor;
    private readonly TwitchStreamStatus _twitchStreamStatus;

    public TwitchStreamStatusTests(ITestOutputHelper output)
        : base(output)
    {
        this._liveStreamMonitor = GetSubstitute<ILiveStreamMonitor>();
        this._liveStreamMonitor.UpdateLiveStreamersAsync().Returns(Task.CompletedTask);

        IMediator mediator = GetSubstitute<IMediator>();

        this._twitchStreamStatus = new TwitchStreamStatus(
            liveStreamMonitor: this._liveStreamMonitor,
            mediator: mediator,
            logger: this.GetTypedLogger<TwitchStreamStatus>()
        );
    }

    [Fact]
    public void CanBeConstructedAndDisposed()
    {
        this._twitchStreamStatus.Dispose();
    }

    [Fact]
    public Task UpdateAsyncWithNoChannelsShouldReturnImmediately()
    {
        return this._twitchStreamStatus.UpdateAsync();
    }

    [Fact]
    public async Task UpdateAsyncShouldRefreshChannelListAfterUnknownStreamerIsRemoved()
    {
        Streamer streamer1 = Streamer.FromString("teststreamer1");
        Streamer streamer2 = Streamer.FromString("teststreamer2");

        string exceptionMessage = $"No channel with the name \"{streamer1.Value}\" could be found.";

        this._liveStreamMonitor.UpdateLiveStreamersAsync()
            .Returns(
                Task.CompletedTask,
                Task.CompletedTask,
                Task.FromException(new InvalidOperationException(exceptionMessage)),
                Task.CompletedTask
            );

        await this._twitchStreamStatus.EnableAsync(streamer1);
        await this._twitchStreamStatus.EnableAsync(streamer2);

        await this._twitchStreamStatus.UpdateAsync();

        await this._twitchStreamStatus.UpdateAsync();

        this._liveStreamMonitor.Received(1)
            .SetChannelsByName(Arg.Is<List<string>>(l => l.Count == 1 && l.Contains(streamer2.Value)));
    }
}
