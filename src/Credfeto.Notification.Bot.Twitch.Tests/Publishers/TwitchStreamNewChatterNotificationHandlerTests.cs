using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using Mediator;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchStreamNewChatterNotificationHandlerTests : TestBase
{
    private readonly INotificationHandler<TwitchStreamNewChatter> _notificationHandler;
    private readonly IShoutoutJoiner _shoutoutJoiner;
    private readonly IUserInfoService _userInfoService;
    private readonly IWelcomeWaggon _welcomeWaggon;

    public TwitchStreamNewChatterNotificationHandlerTests()
    {
        this._userInfoService = GetSubstitute<IUserInfoService>();
        this._welcomeWaggon = GetSubstitute<IWelcomeWaggon>();
        this._shoutoutJoiner = GetSubstitute<IShoutoutJoiner>();

        this._notificationHandler = new TwitchStreamNewChatterNotificationHandler(userInfoService: this._userInfoService,
                                                                                  welcomeWaggon: this._welcomeWaggon,
                                                                                  shoutoutJoiner: this._shoutoutJoiner,
                                                                                  this.GetTypedLogger<TwitchStreamNewChatterNotificationHandler>());
    }

    private Task DidNotReceiveIssueShoutoutAsync()
    {
        return this._shoutoutJoiner.DidNotReceive()
                   .IssueShoutoutAsync(Arg.Any<Streamer>(), Arg.Any<TwitchUser>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }

    private Task ReceivedIssueShoutoutAsync()
    {
        return this._shoutoutJoiner.Received(1)
                   .IssueShoutoutAsync(streamer: MockReferenceData.Streamer,
                                       Arg.Is<TwitchUser>(t => t.UserName == MockReferenceData.Viewer && t.IsStreamer),
                                       Arg.Any<bool>(),
                                       Arg.Any<CancellationToken>());
    }

    private Task DidNotReceiveWelcomeAsync()
    {
        return this._welcomeWaggon.DidNotReceive()
                   .IssueWelcomeAsync(Arg.Any<Streamer>(), Arg.Any<Viewer>(), Arg.Any<CancellationToken>());
    }

    private Task ReceivedWelcomeAsync()
    {
        return this._welcomeWaggon.Received(1)
                   .IssueWelcomeAsync(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, Arg.Any<CancellationToken>());
    }

    private Task<TwitchUser?> ReceivedGetUserAsync()
    {
        return this._userInfoService.Received(1)
                   .GetUserAsync(MockReferenceData.Viewer);
    }

    [Fact]
    public async Task HandleForUnknownUserAsync()
    {
        this._userInfoService.GetUserAsync(MockReferenceData.Viewer)
            .ReturnsNull();

        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, isRegular: false),
                                               cancellationToken: CancellationToken.None);

        await this.ReceivedGetUserAsync();
        await this.DidNotReceiveWelcomeAsync();
        await this.DidNotReceiveIssueShoutoutAsync();
    }

    [Fact]
    public async Task HandleForNewUserAsync()
    {
        this._userInfoService.GetUserAsync(MockReferenceData.Viewer)
            .Returns(new TwitchUser(id: "123456", userName: MockReferenceData.Viewer, isStreamer: false, dateCreated: DateTime.MinValue));

        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, isRegular: false),
                                               cancellationToken: CancellationToken.None);

        await this.ReceivedGetUserAsync();
        await this.DidNotReceiveWelcomeAsync();
        await this.DidNotReceiveIssueShoutoutAsync();
    }

    [Fact]
    public async Task HandleForNewStreamerAsync()
    {
        this._userInfoService.GetUserAsync(MockReferenceData.Viewer)
            .Returns(new TwitchUser(id: "123456", userName: MockReferenceData.Viewer, isStreamer: true, dateCreated: DateTime.MinValue));

        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, isRegular: false),
                                               cancellationToken: CancellationToken.None);

        await this.ReceivedGetUserAsync();
        await this.DidNotReceiveWelcomeAsync();
        await this.ReceivedIssueShoutoutAsync();
    }

    [Fact]
    public async Task HandleForRegularUserAsync()
    {
        this._userInfoService.GetUserAsync(MockReferenceData.Viewer)
            .Returns(new TwitchUser(id: "123456", userName: MockReferenceData.Viewer, isStreamer: false, dateCreated: DateTime.MinValue));

        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, isRegular: true),
                                               cancellationToken: CancellationToken.None);

        await this.ReceivedGetUserAsync();
        await this.ReceivedWelcomeAsync();
        await this.DidNotReceiveIssueShoutoutAsync();
    }

    [Fact]
    public async Task HandleForRegularStreamerAsync()
    {
        this._userInfoService.GetUserAsync(MockReferenceData.Viewer)
            .Returns(new TwitchUser(id: "123456", userName: MockReferenceData.Viewer, isStreamer: true, dateCreated: DateTime.MinValue));

        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, isRegular: true),
                                               cancellationToken: CancellationToken.None);

        await this.ReceivedGetUserAsync();
        await this.ReceivedWelcomeAsync();
        await this.ReceivedIssueShoutoutAsync();
    }

    [Fact]
    public async Task HandleForGetUserErrorAsync()
    {
        this._userInfoService.GetUserAsync(MockReferenceData.Viewer)
            .Throws<ArgumentOutOfRangeException>();

        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, isRegular: true),
                                               cancellationToken: CancellationToken.None);

        await this.ReceivedGetUserAsync();
        await this.DidNotReceiveWelcomeAsync();
        await this.DidNotReceiveIssueShoutoutAsync();
    }
}