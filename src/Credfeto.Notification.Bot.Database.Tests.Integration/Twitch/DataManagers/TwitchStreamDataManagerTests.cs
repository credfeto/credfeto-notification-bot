using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Date.Interfaces;
using Credfeto.Notification.Bot.Database.Tests.Integration.Setup;
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
    public ValueTask AddStreamStartAsync()
    {
        Streamer streamerName = GenerateStreamerUsername();

        return this._twitchStreamDataManager.RecordStreamStartAsync(streamer: streamerName, this._currentTimeSource.UtcNow(), CancellationToken.None);
    }

    [Fact]
    public async Task AddChatterToStreamAsync()
    {
        Streamer streamerName = GenerateStreamerUsername();
        Viewer chatter = GenerateViewerUsername();
        DateTimeOffset streamStart = this._currentTimeSource.UtcNow();

        bool isFirstMessageInStream = await this._twitchStreamDataManager.IsFirstMessageInStreamAsync(streamer: streamerName, streamStartDate: streamStart, username: chatter, CancellationToken.None);
        Assert.True(condition: isFirstMessageInStream, userMessage: "Should be first message");

        await this._twitchStreamDataManager.AddChatterToStreamAsync(streamer: streamerName, streamStartDate: streamStart, username: chatter, CancellationToken.None);

        isFirstMessageInStream = await this._twitchStreamDataManager.IsFirstMessageInStreamAsync(streamer: streamerName, streamStartDate: streamStart, username: chatter, CancellationToken.None);
        Assert.False(condition: isFirstMessageInStream, userMessage: "Should not be first message");
    }

    [Fact]
    public async Task UpdateFollowerMilestoneAsync()
    {
        Streamer streamerName = GenerateStreamerUsername();

        bool isFirstHit = await this._twitchStreamDataManager.UpdateFollowerMilestoneAsync(streamer: streamerName, followerCount: 10, CancellationToken.None);
        Assert.True(condition: isFirstHit, userMessage: "Should be first hit");

        isFirstHit = await this._twitchStreamDataManager.UpdateFollowerMilestoneAsync(streamer: streamerName, followerCount: 10, CancellationToken.None);
        Assert.False(condition: isFirstHit, userMessage: "Should not be first hit");
    }

    [Fact]
    public async Task RecordNewFollowerAsync()
    {
        Streamer streamerName = GenerateStreamerUsername();
        Viewer followerName = GenerateViewerUsername();

        int follows = await this._twitchStreamDataManager.RecordNewFollowerAsync(streamer: streamerName, username: followerName, CancellationToken.None);
        Assert.Equal(expected: 1, actual: follows);

        follows = await this._twitchStreamDataManager.RecordNewFollowerAsync(streamer: streamerName, username: followerName, CancellationToken.None);
        Assert.Equal(expected: 2, actual: follows);
    }

    [Fact]
    public async Task RecordStreamSettingsAsync()
    {
        Streamer streamerName = GenerateStreamerUsername();
        DateTimeOffset streamStart = this._currentTimeSource.UtcNow();

        StreamSettings? settings = await this._twitchStreamDataManager.GetSettingsAsync(streamer: streamerName, streamStartDate: streamStart, CancellationToken.None);
        Assert.Null(settings);

        settings = new(ChatWelcomesEnabled: false, RaidWelcomesEnabled: true, ThanksEnabled: false, AnnounceMilestonesEnabled: false, ShoutOutsEnabled: false);

        await this.UpdateSettingsAsync(streamerName: streamerName, streamStart: streamStart, settings: settings);

        settings = new(ChatWelcomesEnabled: true, RaidWelcomesEnabled: true, ThanksEnabled: true, AnnounceMilestonesEnabled: true, ShoutOutsEnabled: true);

        await this.UpdateSettingsAsync(streamerName: streamerName, streamStart: streamStart, settings: settings);
    }

    private async Task UpdateSettingsAsync(Streamer streamerName, DateTimeOffset streamStart, StreamSettings settings)
    {
        await this._twitchStreamDataManager.UpdateSettingsAsync(streamer: streamerName, streamStartDate: streamStart, settings: settings, CancellationToken.None);

        StreamSettings firstSettings = AssertReallyNotNull(await this._twitchStreamDataManager.GetSettingsAsync(streamer: streamerName, streamStartDate: streamStart, CancellationToken.None));
        Assert.Equal(expected: settings.ThanksEnabled, actual: firstSettings.ThanksEnabled);
        Assert.Equal(expected: settings.ChatWelcomesEnabled, actual: firstSettings.ChatWelcomesEnabled);
        Assert.Equal(expected: settings.RaidWelcomesEnabled, actual: firstSettings.RaidWelcomesEnabled);
        Assert.Equal(expected: settings.AnnounceMilestonesEnabled, actual: firstSettings.AnnounceMilestonesEnabled);
        Assert.Equal(expected: settings.ShoutOutsEnabled, actual: firstSettings.ShoutOutsEnabled);
    }
}