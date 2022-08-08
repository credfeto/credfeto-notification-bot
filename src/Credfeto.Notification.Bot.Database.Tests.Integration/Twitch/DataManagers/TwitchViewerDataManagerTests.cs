using System.Threading.Tasks;
using Credfeto.Notification.Bot.Database.Tests.Integration.Setup;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Xunit;
using Xunit.Abstractions;

namespace Credfeto.Notification.Bot.Database.Tests.Integration.Twitch.DataManagers;

public sealed class TwitchViewerDataManagerTests : DatabaseIntegrationTestBase
{
    private readonly ICurrentTimeSource _currentTimeSource;
    private readonly ITwitchViewerDataManager _twitchStreamerDataManager;

    public TwitchViewerDataManagerTests(ITestOutputHelper output)
        : base(output)
    {
        this._twitchStreamerDataManager = this.GetService<ITwitchViewerDataManager>();
        this._currentTimeSource = this.GetService<ICurrentTimeSource>();
    }

    [Fact]
    public async Task AddAsync()
    {
        Viewer viewerName = GenerateViewerUsername();

        TwitchUser? user = await this._twitchStreamerDataManager.GetByUserNameAsync(viewerName);
        Assert.Null(user);

        await this._twitchStreamerDataManager.AddViewerAsync(viewerName: viewerName, viewerName.ToString(), this._currentTimeSource.UtcNow());

        user = await this._twitchStreamerDataManager.GetByUserNameAsync(viewerName);
        Assert.NotNull(user);
    }
}