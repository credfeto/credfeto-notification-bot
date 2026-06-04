using Credfeto.Notification.Bot.Twitch.DataTypes;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.DataTypes.Tests;

public sealed class StreamerTests : TestBase
{
    [Fact]
    public void FromString_ConvertsToLowercase()
    {
        Streamer streamer = Streamer.FromString("TestStreamer");

        Assert.Equal(expected: "teststreamer", actual: streamer.Value);
    }

    [Fact]
    public void FromString_AlreadyLowercase_ReturnsSameValue()
    {
        Streamer streamer = Streamer.FromString("teststreamer");

        Assert.Equal(expected: "teststreamer", actual: streamer.Value);
    }

    [Fact]
    public void ToViewer_ReturnsViewerWithSameValue()
    {
        Streamer streamer = Streamer.FromString("teststreamer");

        Viewer viewer = streamer.ToViewer();

        Assert.Equal(expected: streamer.Value, actual: viewer.Value);
    }
}
