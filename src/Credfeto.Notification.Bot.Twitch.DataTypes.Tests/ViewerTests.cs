using Credfeto.Notification.Bot.Twitch.DataTypes;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.DataTypes.Tests;

public sealed class ViewerTests : TestBase
{
    [Fact]
    public void FromString_ConvertsToLowercase()
    {
        Viewer viewer = Viewer.FromString("TestViewer");

        Assert.Equal(expected: "testviewer", actual: viewer.Value);
    }

    [Fact]
    public void FromString_AlreadyLowercase_ReturnsSameValue()
    {
        Viewer viewer = Viewer.FromString("testviewer");

        Assert.Equal(expected: "testviewer", actual: viewer.Value);
    }

    [Fact]
    public void ToStreamer_ReturnsStreamerWithSameValue()
    {
        Viewer viewer = Viewer.FromString("testviewer");

        Streamer streamer = viewer.ToStreamer();

        Assert.Equal(expected: viewer.Value, actual: streamer.Value);
    }
}
