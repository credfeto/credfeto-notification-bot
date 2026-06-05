using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.StreamState;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.StreamState;

public sealed class TwitchChatMessageTests : TestBase
{
    [Theory]
    [InlineData("!play", true)]
    [InlineData("/timeout user", true)]
    [InlineData("Hello there", false)]
    [InlineData("regular message", false)]
    [InlineData("", false)]
    public void IsCommandShouldReturnCorrectValue(string message, bool expectedIsCommand)
    {
        TwitchChatMessage chatMessage = new(
            streamer: MockReferenceData.Streamer,
            priority: MessagePriority.NATURAL,
            message: message
        );

        Assert.Equal(expected: expectedIsCommand, actual: chatMessage.IsCommand);
    }

    [Fact]
    public void MessagePropertiesShouldBeSetCorrectly()
    {
        const string message = "!test";

        TwitchChatMessage chatMessage = new(
            streamer: MockReferenceData.Streamer,
            priority: MessagePriority.ASAP,
            message: message
        );

        Assert.Equal(
            expected: (Credfeto.Notification.Bot.Twitch.DataTypes.Streamer)MockReferenceData.Streamer,
            actual: chatMessage.Streamer
        );
        Assert.Equal(expected: MessagePriority.ASAP, actual: chatMessage.Priority);
        Assert.Equal(expected: message, actual: chatMessage.Message);
    }
}
