using System.Threading.Tasks;
using Credfeto.Notification.Bot.Database.Tests.Integration.Setup;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
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
        string channelName = GenerateUsername();

        TwitchUser? user = await this._twitchStreamerDataManager.GetByUserNameAsync(channelName);
        Assert.Null(user);

        await this._twitchStreamerDataManager.AddStreamerAsync(streamerName: channelName, streamerId: channelName, this._currentTimeSource.UtcNow());

        user = await this._twitchStreamerDataManager.GetByUserNameAsync(channelName);
        Assert.NotNull(user);
    }
}