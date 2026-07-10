using System;
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

public sealed class UserInfoServiceTests : LoggingTestBase
{
    private readonly IUserInfoService _userInfoService;

    public UserInfoServiceTests(ITestOutputHelper output)
        : base(output)
    {
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(
            new TwitchBotOptions { Authentication = MockReferenceData.TwitchAuthentication, ChatCommands = [] }
        );

        this._userInfoService = new UserInfoService(options: options, this.GetTypedLogger<UserInfoService>());
    }

    [Fact]
    public Task GetUserPropagatesExceptionOnLookupFailureAsync()
    {
        return Assert.ThrowsAnyAsync<Exception>(() =>
            this._userInfoService.GetUserAsync(userName: MockReferenceData.Viewer, this.CancellationToken())
        );
    }
}
