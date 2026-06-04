using System;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.DataTypes.Tests;

public sealed class TwitchUserTests : TestBase
{
    private static readonly Viewer TestUserName = Viewer.FromString("testuser");
    private static readonly DateTimeOffset TestDateCreated = new(
        year: 2020,
        month: 6,
        day: 15,
        hour: 12,
        minute: 0,
        second: 0,
        offset: TimeSpan.Zero
    );

    [Fact]
    public void Constructor_SetsIdCorrectly()
    {
        TwitchUser user = new(Id: 42, UserName: TestUserName, IsStreamer: false, DateCreated: TestDateCreated);

        Assert.Equal(expected: 42, actual: user.Id);
    }

    [Fact]
    public void Constructor_SetsUserNameCorrectly()
    {
        TwitchUser user = new(Id: 42, UserName: TestUserName, IsStreamer: false, DateCreated: TestDateCreated);

        Assert.Equal(expected: TestUserName, actual: user.UserName);
    }

    [Fact]
    public void Constructor_SetsIsStreamerTrueCorrectly()
    {
        TwitchUser user = new(Id: 42, UserName: TestUserName, IsStreamer: true, DateCreated: TestDateCreated);

        Assert.True(user.IsStreamer, userMessage: "IsStreamer should be true");
    }

    [Fact]
    public void Constructor_SetsIsStreamerFalseCorrectly()
    {
        TwitchUser user = new(Id: 42, UserName: TestUserName, IsStreamer: false, DateCreated: TestDateCreated);

        Assert.False(user.IsStreamer, userMessage: "IsStreamer should be false");
    }

    [Fact]
    public void Constructor_SetsDateCreatedCorrectly()
    {
        TwitchUser user = new(Id: 42, UserName: TestUserName, IsStreamer: false, DateCreated: TestDateCreated);

        Assert.Equal(expected: TestDateCreated, actual: user.DateCreated);
    }
}
