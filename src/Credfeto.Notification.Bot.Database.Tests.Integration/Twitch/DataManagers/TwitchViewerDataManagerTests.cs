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

        TwitchUser? user = await this._twitchStreamerDataManager.GetByUserNameAsync(userName: viewerName, cancellationToken: CancellationToken.None);
        Assert.Null(user);

        await this._twitchStreamerDataManager.AddViewerAsync(viewerName: viewerName,
                                                             Math.Abs(viewerName.GetHashCode()),
                                                             this._currentTimeSource.UtcNow(),
                                                             cancellationToken: CancellationToken.None);

        user = await this._twitchStreamerDataManager.GetByUserNameAsync(userName: viewerName, cancellationToken: CancellationToken.None);
        Assert.NotNull(user);
    }
}