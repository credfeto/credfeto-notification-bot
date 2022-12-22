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

        TwitchUser? user = await this._twitchStreamerDataManager.GetByUserNameAsync(streamerName);
        Assert.Null(user);

        await this._twitchStreamerDataManager.AddStreamerAsync(streamerName: streamerName, streamerName.ToString(), this._currentTimeSource.UtcNow());

        user = await this._twitchStreamerDataManager.GetByUserNameAsync(streamerName);
        Assert.NotNull(user);
    }
}