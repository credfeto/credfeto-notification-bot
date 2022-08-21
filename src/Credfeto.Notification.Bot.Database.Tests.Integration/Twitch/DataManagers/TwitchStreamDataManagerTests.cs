using System;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Database.Tests.Integration.Setup;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Xunit;
using Xunit.Abstractions;

namespace Credfeto.Notification.Bot.Database.Tests.Integration.Twitch.DataManagers;

public sealed class TwitchStreamDataManagerTests : DatabaseIntegrationTestBase
{
    private readonly ICurrentTimeSource _currentTimeSource;
    private readonly ITwitchStreamDataManager _twitchStreamDataManager;

    public TwitchStreamDataManagerTests(ITestOutputHelper output)
        : base(output)
    {
        this._twitchStreamDataManager = this.GetService<ITwitchStreamDataManager>();
        this._currentTimeSource = this.GetService<ICurrentTimeSource>();
    }

    [Fact]
    public Task AddStreamStartAsync()
    {
        Streamer streamerName = GenerateStreamerUsername();

        return this._twitchStreamDataManager.RecordStreamStartAsync(streamer: streamerName, this._currentTimeSource.UtcNow());
    }

    [Fact]
    public async Task AddChatterToStreamAsync()
    {
        Streamer streamerName = GenerateStreamerUsername();
        Viewer chatter = GenerateViewerUsername();
        DateTime streamStart = this._currentTimeSource.UtcNow();

        bool isFirstMessageInStream = await this._twitchStreamDataManager.IsFirstMessageInStreamAsync(streamer: streamerName, streamStartDate: streamStart, username: chatter);
        Assert.True(condition: isFirstMessageInStream, userMessage: "Should be first message");

        await this._twitchStreamDataManager.AddChatterToStreamAsync(streamer: streamerName, streamStartDate: streamStart, username: chatter);

        isFirstMessageInStream = await this._twitchStreamDataManager.IsFirstMessageInStreamAsync(streamer: streamerName, streamStartDate: streamStart, username: chatter);
        Assert.False(condition: isFirstMessageInStream, userMessage: "Should not be first message");
    }

    [Fact]
    public async Task UpdateFollowerMilestoneAsync()
    {
        Streamer streamerName = GenerateStreamerUsername();

        bool isFirstHit = await this._twitchStreamDataManager.UpdateFollowerMilestoneAsync(streamer: streamerName, followerCount: 10);
        Assert.True(condition: isFirstHit, userMessage: "Should be first hit");

        isFirstHit = await this._twitchStreamDataManager.UpdateFollowerMilestoneAsync(streamer: streamerName, followerCount: 10);
        Assert.False(condition: isFirstHit, userMessage: "Should not be first hit");
    }

    [Fact]
    public async Task RecordNewFollowerAsync()
    {
        Streamer streamerName = GenerateStreamerUsername();
        Viewer followerName = GenerateViewerUsername();

        int follows = await this._twitchStreamDataManager.RecordNewFollowerAsync(streamer: streamerName, username: followerName);
        Assert.Equal(expected: 1, actual: follows);

        follows = await this._twitchStreamDataManager.RecordNewFollowerAsync(streamer: streamerName, username: followerName);
        Assert.Equal(expected: 2, actual: follows);
    }

    [Fact]
    public async Task RecordStreamSettingsAsync()
    {
        Streamer streamerName = GenerateStreamerUsername();
        DateTime streamStart = this._currentTimeSource.UtcNow();

        StreamSettings? settings = await this._twitchStreamDataManager.GetSettingsAsync(streamer: streamerName, streamStartDate: streamStart);
        Assert.Null(settings);

        settings = new(chatWelcomesEnabled: false, raidWelcomesEnabled: true, thanksEnabled: false, announceMilestonesEnabled: false, shoutOutsEnabled: false);

        await this._twitchStreamDataManager.UpdateSettingsAsync(streamer: streamerName, streamStartDate: streamStart, settings: settings);

        StreamSettings firstSettings = AssertReallyNotNull(await this._twitchStreamDataManager.GetSettingsAsync(streamer: streamerName, streamStartDate: streamStart));
        Assert.Equal(expected: settings.ThanksEnabled, actual: firstSettings.ThanksEnabled);
        Assert.Equal(expected: settings.ChatWelcomesEnabled, actual: firstSettings.ChatWelcomesEnabled);
        Assert.Equal(expected: settings.RaidWelcomesEnabled, actual: firstSettings.RaidWelcomesEnabled);
        Assert.Equal(expected: settings.AnnounceMilestonesEnabled, actual: firstSettings.AnnounceMilestonesEnabled);
        Assert.Equal(expected: settings.ShoutOutsEnabled, actual: firstSettings.ShoutOutsEnabled);
    }
}