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

public sealed class TwitchStreamerDataManagerTests : DatabaseIntegrationTestBase
{
    private readonly ICurrentTimeSource _currentTimeSource;
    private readonly ITwitchStreamerDataManager _twitchStreamerDataManager;

    public TwitchStreamerDataManagerTests(ITestOutputHelper output)
        : base(output)
    {
        this._twitchStreamerDataManager = this.GetService<ITwitchStreamerDataManager>();
        this._currentTimeSource = this.GetService<ICurrentTimeSource>();
    }

    [Fact]
    public async Task AddAsync()
    {
        Streamer streamerName = GenerateStreamerUsername();

        TwitchUser? user = await this._twitchStreamerDataManager.GetByUserNameAsync(userName: streamerName, cancellationToken: CancellationToken.None);
        Assert.Null(user);

        await this._twitchStreamerDataManager.AddStreamerAsync(streamerName: streamerName,
                                                               Math.Abs(streamerName.GetHashCode()),
                                                               this._currentTimeSource.UtcNow(),
                                                               cancellationToken: CancellationToken.None);

        user = await this._twitchStreamerDataManager.GetByUserNameAsync(userName: streamerName, cancellationToken: CancellationToken.None);
        Assert.NotNull(user);
    }
}