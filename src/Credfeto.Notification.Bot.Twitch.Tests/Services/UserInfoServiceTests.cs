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

public sealed class UserInfoServiceTests : LoggingTestBase
{
    private static readonly DateTimeOffset Now = new(
        year: 2020, month: 1, day: 1, hour: 0, minute: 0, second: 0, offset: TimeSpan.Zero
    );

    private readonly IUserInfoService _userInfoService;

    public UserInfoServiceTests(ITestOutputHelper output)
        : base(output)
    {
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(
            new TwitchBotOptions { Authentication = MockReferenceData.TwitchAuthentication, ChatCommands = [] }
        );

        ICurrentTimeSource currentTimeSource = GetSubstitute<ICurrentTimeSource>();
        currentTimeSource.UtcNow().Returns(Now);

        this._userInfoService = new UserInfoService(
            options: options,
            currentTimeSource: currentTimeSource,
            this.GetTypedLogger<UserInfoService>()
        );
    }

    [Fact]
    public Task GetUserPropagatesExceptionOnLookupFailureAsync()
    {
        return Assert.ThrowsAnyAsync<Exception>(() =>
            this._userInfoService.GetUserAsync(userName: MockReferenceData.Viewer, this.CancellationToken())
        );
    }
}
