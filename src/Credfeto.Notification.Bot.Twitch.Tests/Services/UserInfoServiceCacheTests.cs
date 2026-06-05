using System.Threading.Tasks;
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
    private readonly IUserInfoService _userInfoService;

    public UserInfoServiceCacheTests(ITestOutputHelper output)
        : base(output)
    {
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(
            new TwitchBotOptions { Authentication = MockReferenceData.TwitchAuthentication, ChatCommands = [] }
        );

        this._userInfoService = new UserInfoService(options: options, this.GetTypedLogger<UserInfoService>());
    }

    [Fact]
    public async Task SecondCallForSameViewerShouldReturnCachedResultAsync()
    {
        Viewer viewer = MockReferenceData.Viewer;

        TwitchUser? firstResult = await this._userInfoService.GetUserAsync(userName: viewer, this.CancellationToken());
        TwitchUser? secondResult = await this._userInfoService.GetUserAsync(userName: viewer, this.CancellationToken());

        Assert.Null(firstResult);
        Assert.Null(secondResult);
    }

    [Fact]
    public async Task GetUserAsyncWithStreamerShouldReturnNullIfNotFoundAsync()
    {
        TwitchUser? twitchUser = await this._userInfoService.GetUserAsync(
            userName: MockReferenceData.Streamer,
            this.CancellationToken()
        );

        Assert.Null(twitchUser);
    }
}
