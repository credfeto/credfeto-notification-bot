using System;
using System.Threading.Tasks;
using Credfeto.Date.Interfaces;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Services;

public sealed class UserInfoServiceCacheTests : LoggingTestBase
{
    private static readonly DateTimeOffset Now = new(
        year: 2020, month: 1, day: 1, hour: 0, minute: 0, second: 0, offset: TimeSpan.Zero
    );

    private readonly ICurrentTimeSource _currentTimeSource;
    private readonly IUserInfoService _userInfoService;

    public UserInfoServiceCacheTests(ITestOutputHelper output)
        : base(output)
    {
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(
            new TwitchBotOptions { Authentication = MockReferenceData.TwitchAuthentication, ChatCommands = [] }
        );

        this._currentTimeSource = GetSubstitute<ICurrentTimeSource>();
        this._currentTimeSource.UtcNow().Returns(Now);

        this._userInfoService = new UserInfoService(
            options: options,
            currentTimeSource: this._currentTimeSource,
            this.GetTypedLogger<UserInfoService>()
        );
    }

    [Fact]
    public async Task FailedLookupsAreNotCachedAndEachCallPropagatesAnExceptionAsync()
    {
        Viewer viewer = MockReferenceData.Viewer;

        await Assert.ThrowsAnyAsync<Exception>(() =>
            this._userInfoService.GetUserAsync(userName: viewer, this.CancellationToken())
        );
        await Assert.ThrowsAnyAsync<Exception>(() =>
            this._userInfoService.GetUserAsync(userName: viewer, this.CancellationToken())
        );
    }

    [Fact]
    public Task GetUserAsyncWithStreamerPropagatesExceptionOnLookupFailureAsync()
    {
        return Assert.ThrowsAnyAsync<Exception>(() =>
            this._userInfoService.GetUserAsync(userName: MockReferenceData.Streamer, this.CancellationToken())
        );
    }

    [Fact]
    public async Task GetUserAsyncConsultsTheInjectedCurrentTimeSourceAsync()
    {
        await Assert.ThrowsAnyAsync<Exception>(() =>
            this._userInfoService.GetUserAsync(userName: MockReferenceData.Viewer, this.CancellationToken())
        );

        this._currentTimeSource.Received(1).UtcNow();
    }

    [Fact]
    public Task ConcurrentLookupsForTheSameNotYetCachedViewerAllCompleteAsync()
    {
        Viewer viewer = MockReferenceData.Viewer;

        return Assert.ThrowsAnyAsync<Exception>(() =>
            Task.WhenAll(
                this._userInfoService.GetUserAsync(userName: viewer, this.CancellationToken()),
                this._userInfoService.GetUserAsync(userName: viewer, this.CancellationToken())
            )
        );
    }
}
