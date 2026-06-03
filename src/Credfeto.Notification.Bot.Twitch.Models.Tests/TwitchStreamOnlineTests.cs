using System;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Models.Tests;

public sealed class TwitchStreamOnlineTests : TestBase
{
    private static readonly Streamer TEST_STREAMER = Streamer.FromString("teststreamer");
    private const string TEST_TITLE = "Test Stream Title";
    private const string TEST_GAME_NAME = "Test Game";
    private static readonly DateTime TEST_STARTED_AT = new(
        year: 2024,
        month: 1,
        day: 15,
        hour: 10,
        minute: 0,
        second: 0,
        kind: DateTimeKind.Utc
    );

    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        TwitchStreamOnline online = new(
            streamer: TEST_STREAMER,
            title: TEST_TITLE,
            gameName: TEST_GAME_NAME,
            startedAt: TEST_STARTED_AT
        );

        Assert.Equal(expected: TEST_STREAMER, actual: online.Streamer);
        Assert.Equal(expected: TEST_TITLE, actual: online.Title);
        Assert.Equal(expected: TEST_GAME_NAME, actual: online.GameName);
        Assert.Equal(expected: TEST_STARTED_AT, actual: online.StartedAt);
    }
}
