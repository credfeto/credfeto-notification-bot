using System;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Database.Tests.Integration.Setup;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
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
        string channelName = GenerateUsername();

        return this._twitchStreamDataManager.RecordStreamStartAsync(channel: channelName, this._currentTimeSource.UtcNow());
    }

    [Fact]
    public async Task AddChatterToStreamAsync()
    {
        string channelName = GenerateUsername();
        string chatter = GenerateUsername();
        DateTime streamStart = this._currentTimeSource.UtcNow();

        bool isFirstMessageInStream = await this._twitchStreamDataManager.IsFirstMessageInStreamAsync(channel: channelName, streamStartDate: streamStart, username: chatter);
        Assert.True(condition: isFirstMessageInStream, userMessage: "Should be first message");

        await this._twitchStreamDataManager.AddChatterToStreamAsync(channel: channelName, streamStartDate: streamStart, username: chatter);

        isFirstMessageInStream = await this._twitchStreamDataManager.IsFirstMessageInStreamAsync(channel: channelName, streamStartDate: streamStart, username: chatter);
        Assert.False(condition: isFirstMessageInStream, userMessage: "Should not be first message");
    }
}