using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using MediatR;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchStreamOfflineNotificationHandlerTests : TestBase
{
    private static readonly Streamer OtherStreamer = MockReferenceData.Streamer.Next();

    private readonly INotificationHandler<TwitchStreamOffline> _notificationHandler;
    private readonly ITwitchChannelManager _twitchChannelManager;
    private readonly ITwitchChannelState _twitchChannelState;

    public TwitchStreamOfflineNotificationHandlerTests()
    {
        this._twitchChannelManager = GetSubstitute<ITwitchChannelManager>();
        this._twitchChannelState = GetSubstitute<ITwitchChannelState>();

        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions(authentication: MockReferenceData.TwitchAuthentication,
                                                   milestones: MockReferenceData.TwitchMilestones,
                                                   ignoredUsers: MockReferenceData.IgnoredUsers,
                                                   heists: MockReferenceData.Heists,
                                                   marbles: null,
                                                   channels: new()
                                                             {
                                                                 new(channelName: ((Streamer)MockReferenceData.Streamer).Value,
                                                                     shoutOuts: MockReferenceData.TwitchChannelShoutout,
                                                                     raids: MockReferenceData.TwitchChannelRaids,
                                                                     thanks: MockReferenceData.TwitchChannelThanks,
                                                                     mileStones: MockReferenceData.TwitchChanelMileStone,
                                                                     welcome: MockReferenceData.TwitchChannelWelcome)
                                                             }));

        this._notificationHandler = new TwitchStreamOfflineNotificationHandler(options: options,
                                                                               twitchChannelManager: this._twitchChannelManager,
                                                                               this.GetTypedLogger<TwitchStreamOfflineNotificationHandler>());
    }

    [Fact]
    public async Task HandleModChannelAsync()
    {
        this._twitchChannelManager.GetStreamer(MockReferenceData.Streamer)
            .Returns(this._twitchChannelState);

        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, title: "Skydiving", gameName: "IRL", new(year: 2020, month: 1, day: 1)),
                                               cancellationToken: CancellationToken.None);

        this._twitchChannelManager.Received(1)
            .GetStreamer(MockReferenceData.Streamer);

        this._twitchChannelState.Received(1)
            .Offline();
    }

    [Fact]
    public async Task HandleModChannelExceptionAsync()
    {
        this._twitchChannelManager.GetStreamer(MockReferenceData.Streamer)
            .Returns(this._twitchChannelState);

        this._twitchChannelState.When(x => x.Offline())
            .Do(_ => throw new ArithmeticException());

        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, title: "Skydiving", gameName: "IRL", new(year: 2020, month: 1, day: 1)),
                                               cancellationToken: CancellationToken.None);

        this._twitchChannelManager.Received(1)
            .GetStreamer(MockReferenceData.Streamer);

        this._twitchChannelState.Received(1)
            .Offline();
    }

    [Fact]
    public async Task HandleOtherChannelAsync()
    {
        await this._notificationHandler.Handle(new(streamer: OtherStreamer, title: "Skydiving", gameName: "IRL", new(year: 2020, month: 1, day: 1)),
                                               cancellationToken: CancellationToken.None);

        this._twitchChannelManager.DidNotReceive()
            .GetStreamer(Arg.Any<Streamer>());

        this._twitchChannelState.DidNotReceive()
            .Offline();
    }
}