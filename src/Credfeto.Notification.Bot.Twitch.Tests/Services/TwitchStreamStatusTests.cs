using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using Mediator;
using NSubstitute;
using NSubstitute.Extensions;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using Xunit;
using HelixStream = TwitchLib.Api.Helix.Models.Streams.GetStreams.Stream;

namespace Credfeto.Notification.Bot.Twitch.Tests.Services;

public sealed class TwitchStreamStatusTests : LoggingTestBase
{
    private readonly ILiveStreamMonitor _liveStreamMonitor;
    private readonly IMediator _mediator;
    private readonly TwitchStreamStatus _twitchStreamStatus;

    public TwitchStreamStatusTests(ITestOutputHelper output)
        : base(output)
    {
        this._liveStreamMonitor = GetSubstitute<ILiveStreamMonitor>();
        this._liveStreamMonitor.UpdateLiveStreamersAsync().Returns(Task.CompletedTask);

        this._mediator = GetSubstitute<IMediator>();

        this._twitchStreamStatus = new TwitchStreamStatus(
            liveStreamMonitor: this._liveStreamMonitor,
            mediator: this._mediator,
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

    [Fact]
    public async Task UpdateAsyncShouldNotLoseVersionBumpFromConcurrentEnableAsync()
    {
        Streamer streamer1 = Streamer.FromString("teststreamer1");
        Streamer streamer2 = Streamer.FromString("teststreamer2");

        Task? reentrantEnableTask = null;

        this._liveStreamMonitor.When(x => x.SetChannelsByName(Arg.Any<List<string>>()))
            .Do(_ =>
            {
                reentrantEnableTask ??= this._twitchStreamStatus.EnableAsync(streamer2).AsTask();
            });

        await this
            ._twitchStreamStatus.EnableAsync(streamer1)
            .AsTask()
            .WaitAsync(timeout: TimeSpan.FromSeconds(10), cancellationToken: this.CancellationToken());

        Assert.NotNull(reentrantEnableTask);

        await reentrantEnableTask.WaitAsync(
            timeout: TimeSpan.FromSeconds(10),
            cancellationToken: this.CancellationToken()
        );

        this._liveStreamMonitor.Received(1)
            .SetChannelsByName(Arg.Is<List<string>>(l => l.Count == 1 && l.Contains(streamer1.Value)));

        this._liveStreamMonitor.Received(1)
            .SetChannelsByName(
                Arg.Is<List<string>>(l => l.Count == 2 && l.Contains(streamer1.Value) && l.Contains(streamer2.Value))
            );
    }

    [Fact]
    public Task UpdateAsyncShouldRethrowUnrecognisedInvalidOperationException()
    {
        Streamer streamer = Streamer.FromString("teststreamer1");

        this._liveStreamMonitor.UpdateLiveStreamersAsync()
            .Returns(_ => Task.FromException(new InvalidOperationException("Something else went wrong")));

        return Assert.ThrowsAsync<InvalidOperationException>(() =>
            this._twitchStreamStatus.EnableAsync(streamer).AsTask()
        );
    }

    [Fact]
    public async Task StreamOnlineEventShouldPublishTwitchStreamOnline()
    {
        this._liveStreamMonitor.OnStreamOnline += Raise.EventWith(
            new OnStreamOnlineArgs { Channel = "teststreamer1", Stream = CreateStream() }
        );

        await this
            ._mediator.Received(1)
            .Publish(
                Arg.Is<TwitchStreamOnline>(n =>
                    n.Streamer == Streamer.FromString("teststreamer1")
                    && n.Title == "Test Title"
                    && n.GameName == "Test Game"
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task StreamOnlineEventShouldSwallowPublishFailure()
    {
        // Configured on the substitute rather than the Publish call so no ValueTask is consumed outside an await (CA2012).
        this._mediator.ReturnsForAll<ValueTask>(_ =>
            ValueTask.FromException(new InvalidOperationException("Publish failed"))
        );

        this._liveStreamMonitor.OnStreamOnline += Raise.EventWith(
            new OnStreamOnlineArgs { Channel = "teststreamer1", Stream = CreateStream() }
        );

        await this._mediator.Received(1).Publish(Arg.Any<TwitchStreamOnline>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task StreamOfflineEventShouldPublishTwitchStreamOffline()
    {
        this._liveStreamMonitor.OnStreamOffline += Raise.EventWith(
            new OnStreamOfflineArgs { Channel = "teststreamer1", Stream = CreateStream() }
        );

        await this
            ._mediator.Received(1)
            .Publish(
                Arg.Is<TwitchStreamOffline>(n =>
                    n.Streamer == Streamer.FromString("teststreamer1")
                    && n.Title == "Test Title"
                    && n.GameName == "Test Game"
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task StreamOfflineEventShouldSwallowPublishFailure()
    {
        // Configured on the substitute rather than the Publish call so no ValueTask is consumed outside an await (CA2012).
        this._mediator.ReturnsForAll<ValueTask>(_ =>
            ValueTask.FromException(new InvalidOperationException("Publish failed"))
        );

        this._liveStreamMonitor.OnStreamOffline += Raise.EventWith(
            new OnStreamOfflineArgs { Channel = "teststreamer1", Stream = CreateStream() }
        );

        await this._mediator.Received(1).Publish(Arg.Any<TwitchStreamOffline>(), Arg.Any<CancellationToken>());
    }

    private static HelixStream CreateStream()
    {
        return new TestStream(
            title: "Test Title",
            gameName: "Test Game",
            startedAt: new(year: 2020, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc)
        );
    }

    // TwitchLib only exposes protected setters on the Helix stream model, so a subclass is needed to populate it.
    private sealed class TestStream : HelixStream
    {
        public TestStream(string title, string gameName, in DateTime startedAt)
        {
            this.Title = title;
            this.GameName = gameName;
            this.StartedAt = startedAt;
        }
    }
}
