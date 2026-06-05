using System;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.StreamState;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.StreamState;

public sealed class TwitchChannelStateTests : LoggingTestBase
{
    private static readonly DateTimeOffset TestStartDate = new(
        year: 2024,
        month: 1,
        day: 1,
        hour: 12,
        minute: 0,
        second: 0,
        offset: TimeSpan.Zero
    );

    private readonly ITwitchChannelState _channelState;

    public TwitchChannelStateTests(ITestOutputHelper output)
        : base(output)
    {
        this._channelState = new TwitchChannelState(
            streamerStreamer: MockReferenceData.Streamer,
            logger: this.GetTypedLogger<TwitchChannelState>()
        );
    }

    [Fact]
    public void InitiallyOffline()
    {
        Assert.False(condition: this._channelState.IsOnline, userMessage: "Should start offline");
    }

    [Fact]
    public void InitiallyNotChatted()
    {
        Assert.False(condition: this._channelState.Chatted, userMessage: "Should not have chatted initially");
    }

    [Fact]
    public async Task OnlineAsyncShouldSetOnline()
    {
        await this._channelState.OnlineAsync(
            gameName: "TestGame",
            startDate: TestStartDate,
            cancellationToken: this.CancellationToken()
        );

        Assert.True(condition: this._channelState.IsOnline, userMessage: "Should be online after OnlineAsync");
    }

    [Fact]
    public async Task OfflineShouldSetOffline()
    {
        await this._channelState.OnlineAsync(
            gameName: "TestGame",
            startDate: TestStartDate,
            cancellationToken: this.CancellationToken()
        );

        this._channelState.Offline();

        Assert.False(condition: this._channelState.IsOnline, userMessage: "Should be offline after Offline()");
    }

    [Fact]
    public async Task ChattedShouldBeTrueWhenSetAfterGoingOnline()
    {
        await this._channelState.OnlineAsync(
            gameName: "TestGame",
            startDate: TestStartDate,
            cancellationToken: this.CancellationToken()
        );

        this._channelState.Chatted = true;

        Assert.True(condition: this._channelState.Chatted, userMessage: "Chatted should be true after being set");
    }

    [Fact]
    public async Task ChattedShouldBeFalseWhenChannelGoesOffline()
    {
        await this._channelState.OnlineAsync(
            gameName: "TestGame",
            startDate: TestStartDate,
            cancellationToken: this.CancellationToken()
        );

        this._channelState.Chatted = true;
        this._channelState.Offline();

        Assert.False(condition: this._channelState.Chatted, userMessage: "Chatted should be false after going offline");
    }

    [Fact]
    public void SetChattedWhenOfflineShouldNotThrow()
    {
        this._channelState.Chatted = true;

        Assert.False(
            condition: this._channelState.Chatted,
            userMessage: "Chatted should remain false when setting while offline"
        );
    }

    [Fact]
    public void StreamerShouldMatchProvided()
    {
        Assert.Equal(
            expected: (Credfeto.Notification.Bot.Twitch.DataTypes.Streamer)MockReferenceData.Streamer,
            actual: this._channelState.Streamer
        );
    }
}
