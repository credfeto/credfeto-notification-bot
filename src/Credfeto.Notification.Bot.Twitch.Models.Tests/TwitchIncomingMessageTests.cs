using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Models.Tests;

public sealed class TwitchIncomingMessageTests : TestBase
{
    private static readonly Streamer TEST_STREAMER = Streamer.FromString("teststreamer");
    private static readonly Viewer TEST_CHATTER = Viewer.FromString("testchatter");
    private const string TEST_MESSAGE = "Hello world";

    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        TwitchIncomingMessage message = new(Streamer: TEST_STREAMER, Chatter: TEST_CHATTER, Message: TEST_MESSAGE);

        Assert.Equal(expected: TEST_STREAMER, actual: message.Streamer);
        Assert.Equal(expected: TEST_CHATTER, actual: message.Chatter);
        Assert.Equal(expected: TEST_MESSAGE, actual: message.Message);
    }

    [Fact]
    public void EqualRecords_AreEqual()
    {
        TwitchIncomingMessage first = new(Streamer: TEST_STREAMER, Chatter: TEST_CHATTER, Message: TEST_MESSAGE);
        TwitchIncomingMessage second = new(Streamer: TEST_STREAMER, Chatter: TEST_CHATTER, Message: TEST_MESSAGE);

        Assert.True(condition: first.Equals(second), userMessage: "Records with same values should be equal");
        Assert.True(condition: first == second, userMessage: "Records with same values should be equal via ==");
        Assert.False(condition: first != second, userMessage: "Records with same values should not be unequal via !=");
    }

    [Fact]
    public void DifferentRecords_AreNotEqual()
    {
        Streamer otherStreamer = Streamer.FromString("otherstreamer");
        TwitchIncomingMessage first = new(Streamer: TEST_STREAMER, Chatter: TEST_CHATTER, Message: TEST_MESSAGE);
        TwitchIncomingMessage second = new(Streamer: otherStreamer, Chatter: TEST_CHATTER, Message: TEST_MESSAGE);

        Assert.False(
            condition: first.Equals(second),
            userMessage: "Records with different streamers should not be equal"
        );
        Assert.False(
            condition: first == second,
            userMessage: "Records with different streamers should not be equal via =="
        );
        Assert.True(
            condition: first != second,
            userMessage: "Records with different streamers should be unequal via !="
        );
    }

    [Fact]
    public void GetHashCode_IsConsistentForEqualValues()
    {
        TwitchIncomingMessage first = new(Streamer: TEST_STREAMER, Chatter: TEST_CHATTER, Message: TEST_MESSAGE);
        TwitchIncomingMessage second = new(Streamer: TEST_STREAMER, Chatter: TEST_CHATTER, Message: TEST_MESSAGE);

        Assert.Equal(expected: first.GetHashCode(), actual: second.GetHashCode());
    }
}
