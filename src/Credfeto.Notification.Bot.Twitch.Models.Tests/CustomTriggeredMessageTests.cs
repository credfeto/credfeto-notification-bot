using System;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Models.Tests;

public sealed class CustomTriggeredMessageTests : TestBase
{
    private static readonly Streamer TEST_STREAMER = Streamer.FromString("teststreamer");
    private const string TEST_MESSAGE = "Hello world";

    private static string GetNullString()
    {
        // ! intentional null for testing constructor null guards
        return null!;
    }

    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        CustomTriggeredMessage message = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);

        Assert.Equal(expected: TEST_STREAMER, actual: message.Streamer);
        Assert.Equal(expected: TEST_MESSAGE, actual: message.Message);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenMessageIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new CustomTriggeredMessage(streamer: TEST_STREAMER, message: GetNullString())
        );
    }
}
