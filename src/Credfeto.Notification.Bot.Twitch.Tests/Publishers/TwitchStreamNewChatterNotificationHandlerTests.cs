using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using MediatR;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchStreamNewChatterNotificationHandlerTests : TestBase
{
    private static readonly Channel Channel = Types.ChannelFromString(nameof(Channel));
    private static readonly User User = Types.UserFromString(nameof(User));
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
                   .IssueShoutoutAsync(Arg.Any<Channel>(), Arg.Any<TwitchUser>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }

    private Task ReceivedIssueShoutoutAsync()
    {
        return this._shoutoutJoiner.Received(1)
                   .IssueShoutoutAsync(channel: Channel, Arg.Is<TwitchUser>(t => t.UserName == User && t.IsStreamer), Arg.Any<bool>(), Arg.Any<CancellationToken>());
    }

    private Task DidNotReceiveWelcomeAsync()
    {
        return this._welcomeWaggon.DidNotReceive()
                   .IssueWelcomeAsync(Arg.Any<Channel>(), Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    private Task ReceivedWelcomeAsync()
    {
        return this._welcomeWaggon.Received(1)
                   .IssueWelcomeAsync(channel: Channel, user: User, Arg.Any<CancellationToken>());
    }

    private Task<TwitchUser?> ReceivedGetUserAsync()
    {
        return this._userInfoService.Received(1)
                   .GetUserAsync(User);
    }

    [Fact]
    public async Task HandleForUnknownUserAsync()
    {
        this._userInfoService.GetUserAsync(User)
            .ReturnsNull();

        await this._notificationHandler.Handle(new(channel: Channel, user: User, isRegular: false), cancellationToken: CancellationToken.None);

        await this.ReceivedGetUserAsync();
        await this.DidNotReceiveWelcomeAsync();
        await this.DidNotReceiveIssueShoutoutAsync();
    }

    [Fact]
    public async Task HandleForNewUserAsync()
    {
        this._userInfoService.GetUserAsync(User)
            .Returns(new TwitchUser(id: "123456", userName: User, isStreamer: false, dateCreated: DateTime.MinValue));

        await this._notificationHandler.Handle(new(channel: Channel, user: User, isRegular: false), cancellationToken: CancellationToken.None);

        await this.ReceivedGetUserAsync();
        await this.DidNotReceiveWelcomeAsync();
        await this.DidNotReceiveIssueShoutoutAsync();
    }

    [Fact]
    public async Task HandleForNewStreamerAsync()
    {
        this._userInfoService.GetUserAsync(User)
            .Returns(new TwitchUser(id: "123456", userName: User, isStreamer: true, dateCreated: DateTime.MinValue));

        await this._notificationHandler.Handle(new(channel: Channel, user: User, isRegular: false), cancellationToken: CancellationToken.None);

        await this.ReceivedGetUserAsync();
        await this.DidNotReceiveWelcomeAsync();
        await this.ReceivedIssueShoutoutAsync();
    }

    [Fact]
    public async Task HandleForRegularUserAsync()
    {
        this._userInfoService.GetUserAsync(User)
            .Returns(new TwitchUser(id: "123456", userName: User, isStreamer: false, dateCreated: DateTime.MinValue));

        await this._notificationHandler.Handle(new(channel: Channel, user: User, isRegular: true), cancellationToken: CancellationToken.None);

        await this.ReceivedGetUserAsync();
        await this.ReceivedWelcomeAsync();
        await this.DidNotReceiveIssueShoutoutAsync();
    }

    [Fact]
    public async Task HandleForRegularStreamerAsync()
    {
        this._userInfoService.GetUserAsync(User)
            .Returns(new TwitchUser(id: "123456", userName: User, isStreamer: true, dateCreated: DateTime.MinValue));

        await this._notificationHandler.Handle(new(channel: Channel, user: User, isRegular: true), cancellationToken: CancellationToken.None);

        await this.ReceivedGetUserAsync();
        await this.ReceivedWelcomeAsync();
        await this.ReceivedIssueShoutoutAsync();
    }
}