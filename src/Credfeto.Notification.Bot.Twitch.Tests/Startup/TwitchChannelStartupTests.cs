using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Startup;
using Credfeto.Services.Startup.Interfaces;
using FunFair.Test.Common;
using Mediator;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Startup;

public sealed class TwitchChannelStartupTests : LoggingTestBase
{
    private static readonly DateTimeOffset TestDate = new(
        year: 2024,
        month: 1,
        day: 1,
        hour: 12,
        minute: 0,
        second: 0,
        offset: TimeSpan.Zero
    );

    private static readonly TwitchBotOptions OptionsWithBotOnly = new()
    {
        Authentication = new()
        {
            Api = new() { ClientId = "client-id", ClientSecret = "client-secret" },
            Chat = new() { UserName = "botuser", OAuthToken = "oauth-token" },
        },
        ChatCommands = [],
    };

    private static readonly TwitchBotOptions OptionsWithStreamer = new()
    {
        Authentication = new()
        {
            Api = new() { ClientId = "client-id", ClientSecret = "client-secret" },
            Chat = new() { UserName = "botuser", OAuthToken = "oauth-token" },
        },
        ChatCommands =
        [
            new(streamer: "teststreamer", bot: "botuser", match: "!play", issue: "!play", matchType: "EXACT"),
        ],
    };

    private readonly IMediator _mediator;
    private readonly IRunOnStartup _startup;
    private readonly ITwitchChat _twitchChat;
    private readonly IUserInfoService _userInfoService;

    public TwitchChannelStartupTests(ITestOutputHelper output)
        : base(output)
    {
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(OptionsWithBotOnly);

        this._userInfoService = GetSubstitute<IUserInfoService>();
        this._twitchChat = GetSubstitute<ITwitchChat>();
        this._mediator = GetSubstitute<IMediator>();

        this._startup = new TwitchChannelStartup(
            options: options,
            userInfoService: this._userInfoService,
            twitchChat: this._twitchChat,
            mediator: this._mediator,
            logger: this.GetTypedLogger<TwitchChannelStartup>()
        );
    }

    [Fact]
    public async Task StartAsyncShouldJoinBotChannelAndPublishWatchChannelWhenUserFound()
    {
        TwitchUser user = new(Id: 42, UserName: Viewer.FromString("botuser"), IsStreamer: false, DateCreated: TestDate);

        this._userInfoService.GetUserAsync(Arg.Any<Streamer>(), Arg.Any<CancellationToken>()).Returns(user);

        await this._startup.StartAsync(this.CancellationToken());

        this._twitchChat.Received(1).JoinChat(Arg.Any<Streamer>());

        await this._mediator.Received(1).Publish(Arg.Any<TwitchWatchChannel>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task StartAsyncWithStreamersInChatCommandsShouldPublishMultipleWatchChannels()
    {
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(OptionsWithStreamer);

        IUserInfoService userInfoService = GetSubstitute<IUserInfoService>();
        ITwitchChat twitchChat = GetSubstitute<ITwitchChat>();
        IMediator mediator = GetSubstitute<IMediator>();

        IRunOnStartup startup = new TwitchChannelStartup(
            options: options,
            userInfoService: userInfoService,
            twitchChat: twitchChat,
            mediator: mediator,
            logger: this.GetTypedLogger<TwitchChannelStartup>()
        );

        TwitchUser user = new(Id: 42, UserName: Viewer.FromString("botuser"), IsStreamer: false, DateCreated: TestDate);
        userInfoService.GetUserAsync(Arg.Any<Streamer>(), Arg.Any<CancellationToken>()).Returns(user);

        await startup.StartAsync(this.CancellationToken());

        twitchChat.Received(1).JoinChat(Arg.Any<Streamer>());
    }

    [Fact]
    public async Task StartAsyncShouldNotPublishWatchChannelWhenUserNotFound()
    {
        this._userInfoService.GetUserAsync(Arg.Any<Streamer>(), Arg.Any<CancellationToken>())
            .Returns((TwitchUser?)null);

        await this._startup.StartAsync(this.CancellationToken());

        this._twitchChat.Received(1).JoinChat(Arg.Any<Streamer>());

        await this._mediator.DidNotReceive().Publish(Arg.Any<TwitchWatchChannel>(), Arg.Any<CancellationToken>());
    }
}
