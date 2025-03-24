using System.Threading;
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

public sealed class UserInfoServiceTests : TestBase
{
    private readonly IUserInfoService _userInfoService;

    public UserInfoServiceTests()
    {
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(
            new TwitchBotOptions(authentication: MockReferenceData.TwitchAuthentication, [])
        );

        this._userInfoService = new UserInfoService(
            options: options,
            this.GetTypedLogger<UserInfoService>()
        );
    }

    [Fact]
    public async Task GetUserReturnsNullIfNotFoundAsync()
    {
        TwitchUser? twitchUser = await this._userInfoService.GetUserAsync(
            userName: MockReferenceData.Viewer,
            cancellationToken: this.CancellationToken()
        );

        Assert.Null(twitchUser);
    }
}
