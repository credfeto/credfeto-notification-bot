using System;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Services;

public sealed class UserInfoServiceTests : TestBase
{
    private static readonly Viewer Username = Viewer.FromString(nameof(Username));
    private readonly ITwitchStreamerDataManager _twitchStreamerDataManager;
    private readonly IUserInfoService _userInfoService;

    public UserInfoServiceTests()
    {
        this._twitchStreamerDataManager = GetSubstitute<ITwitchStreamerDataManager>();

        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions { Authentication = new() { ClientId = "Invalid", ClientSecret = "Invalid", ClientAccessToken = "Invalid" } });
        this._userInfoService = new UserInfoService(options: options, twitchStreamerDataManager: this._twitchStreamerDataManager, this.GetTypedLogger<UserInfoService>());
    }

    [Fact]
    public async Task GetUserReturnsNullIfNotFoundAsync()
    {
        TwitchUser? twitchUser = await this._userInfoService.GetUserAsync(Username);

        Assert.Null(twitchUser);

        await this._twitchStreamerDataManager.Received(1)
                  .GetByUserNameAsync(Username);

        await this._twitchStreamerDataManager.DidNotReceive()
                  .AddStreamerAsync(Arg.Any<Streamer>(), Arg.Any<string>(), Arg.Any<DateTime>());
    }
}