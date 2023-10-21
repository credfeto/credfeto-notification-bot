using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
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
    private readonly ITwitchStreamerDataManager _twitchStreamerDataManager;
    private readonly ITwitchViewerDataManager _twitchViewerDataManager;
    private readonly IUserInfoService _userInfoService;

    public UserInfoServiceTests()
    {
        this._twitchStreamerDataManager = GetSubstitute<ITwitchStreamerDataManager>();
        this._twitchViewerDataManager = GetSubstitute<ITwitchViewerDataManager>();

        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions(authentication: MockReferenceData.TwitchAuthentication,
                                                   milestones: MockReferenceData.TwitchMilestones,
                                                   ignoredUsers: MockReferenceData.IgnoredUsers,
                                                   chatCommands: Array.Empty<TwitchChatCommand>(),
                                                   channels: Array.Empty<TwitchModChannel>()));

        this._userInfoService = new UserInfoService(options: options,
                                                    twitchStreamerDataManager: this._twitchStreamerDataManager,
                                                    twitchViewerDataManager: this._twitchViewerDataManager,
                                                    this.GetTypedLogger<UserInfoService>());
    }

    [Fact]
    public async Task GetUserReturnsNullIfNotFoundAsync()
    {
        TwitchUser? twitchUser = await this._userInfoService.GetUserAsync(userName: MockReferenceData.Viewer, cancellationToken: CancellationToken.None);

        Assert.Null(twitchUser);

        await this._twitchStreamerDataManager.Received(1)
                  .GetByUserNameAsync(userName: MockReferenceData.Viewer, Arg.Any<CancellationToken>());

        await this._twitchStreamerDataManager.DidNotReceive()
                  .AddStreamerAsync(Arg.Any<Streamer>(), Arg.Any<int>(), Arg.Any<DateTimeOffset>(), Arg.Any<CancellationToken>());
    }
}