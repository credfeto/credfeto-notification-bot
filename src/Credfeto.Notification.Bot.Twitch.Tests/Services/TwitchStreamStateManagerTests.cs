using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Services;

public sealed class TwitchStreamStateManagerTests : LoggingTestBase
{
    private readonly ITwitchStreamStateManager _manager;

    public TwitchStreamStateManagerTests(ITestOutputHelper output)
        : base(output)
    {
        this._manager = new TwitchStreamStateManager(this.GetTypedLogger<TwitchStreamStateManager>());
    }

    [Fact]
    public void GetShouldReturnStateForNewStreamer()
    {
        Streamer streamer = MockReferenceData.Streamer;

        ITwitchChannelState state = this._manager.Get(streamer);

        Assert.NotNull(state);
        Assert.Equal(expected: streamer, actual: state.Streamer);
    }

    [Fact]
    public void GetShouldReturnSameStateForSameStreamer()
    {
        Streamer streamer = MockReferenceData.Streamer;

        ITwitchChannelState state1 = this._manager.Get(streamer);
        ITwitchChannelState state2 = this._manager.Get(streamer);

        Assert.Same(expected: state1, actual: state2);
    }

    [Fact]
    public void GetShouldReturnDifferentStateForDifferentStreamers()
    {
        Streamer streamer1 = MockReferenceData.Streamer;
        Streamer streamer2 = MockReferenceData.Streamer.Next();

        ITwitchChannelState state1 = this._manager.Get(streamer1);
        ITwitchChannelState state2 = this._manager.Get(streamer2);

        Assert.NotSame(state1, state2);
    }

    [Fact]
    public void InitialStateIsOffline()
    {
        Streamer streamer = MockReferenceData.Streamer;

        ITwitchChannelState state = this._manager.Get(streamer);

        Assert.False(condition: state.IsOnline, userMessage: "New streamer state should be offline");
    }
}
