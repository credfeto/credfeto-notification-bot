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
using Mediator;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchStreamOnlineNotificationHandlerTests : TestBase
{
    private static readonly Streamer NonStreamer = MockReferenceData.Streamer.Next();

    private readonly INotificationHandler<TwitchStreamOnline> _notificationHandler;

    private readonly ITwitchChannelManager _twitchChannelManager;
    private readonly ITwitchChannelState _twitchChannelState;

    public TwitchStreamOnlineNotificationHandlerTests()
    {
        this._twitchChannelManager = GetSubstitute<ITwitchChannelManager>();
        this._twitchChannelState = GetSubstitute<ITwitchChannelState>();

        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions(authentication: MockReferenceData.TwitchAuthentication,
                                                   milestones: MockReferenceData.TwitchMilestones,
                                                   ignoredUsers: MockReferenceData.IgnoredUsers,
                                                   chatCommands: Array.Empty<TwitchChatCommand>(),
                                                   channels: new TwitchModChannel[]
                                                             {
                                                                 new(channelName: ((Streamer)MockReferenceData.Streamer).Value,
                                                                     thanks: MockReferenceData.TwitchChannelThanks,
                                                                     shoutOuts: MockReferenceData.TwitchChannelShoutout,
                                                                     raids: MockReferenceData.TwitchChannelRaids,
                                                                     mileStones: MockReferenceData.TwitchChanelMileStone,
                                                                     welcome: MockReferenceData.TwitchChannelWelcome)
                                                             }));

        this._notificationHandler = new TwitchStreamOnlineNotificationHandler(options: options,
                                                                              twitchChannelManager: this._twitchChannelManager,
                                                                              this.GetTypedLogger<TwitchStreamOnlineNotificationHandler>());
    }

    [Fact]
    public async Task HandleWhenModChannelAsync()
    {
        this._twitchChannelManager.GetStreamer(MockReferenceData.Streamer)
            .Returns(this._twitchChannelState);

        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer,
                                                   title: "Skydiving",
                                                   gameName: "IRL",
                                                   new(year: 2020, month: 1, day: 1, hour: 0, minute: 0, second: 0, millisecond: 0, kind: DateTimeKind.Utc)),
                                               cancellationToken: CancellationToken.None);

        this._twitchChannelManager.Received(1)
            .GetStreamer(MockReferenceData.Streamer);

        await this._twitchChannelState.Received(1)
                  .OnlineAsync(gameName: "IRL", new(year: 2020, month: 1, day: 1, hour: 0, minute: 0, second: 0, millisecond: 0, offset: TimeSpan.Zero), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleWhenModChannelExceptionAsync()
    {
        this._twitchChannelManager.GetStreamer(MockReferenceData.Streamer)
            .Returns(this._twitchChannelState);

        this._twitchChannelState.When(async x => await x.OnlineAsync(Arg.Any<string>(), Arg.Any<DateTimeOffset>(), Arg.Any<CancellationToken>()))
            .Do(_ => throw new TimeoutException());

        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer,
                                                   title: "Skydiving",
                                                   gameName: "IRL",
                                                   new(year: 2020, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc)),
                                               cancellationToken: CancellationToken.None);

        this._twitchChannelManager.Received(1)
            .GetStreamer(MockReferenceData.Streamer);

        await this._twitchChannelState.Received(1)
                  .OnlineAsync(gameName: "IRL", new(year: 2020, month: 1, day: 1, hour: 0, minute: 0, second: 0, offset: TimeSpan.Zero), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleWhenNotModChannelAsync()
    {
        this._twitchChannelManager.GetStreamer(Arg.Any<Streamer>())
            .Returns(this._twitchChannelState);

        await this._notificationHandler.Handle(
            new(streamer: NonStreamer, title: "Skydiving", gameName: "IRL", new(year: 2020, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc)),
            cancellationToken: CancellationToken.None);

        this._twitchChannelManager.DidNotReceive()
            .GetStreamer(Arg.Any<Streamer>());

        await this._twitchChannelState.DidNotReceive()
                  .OnlineAsync(Arg.Any<string>(), Arg.Any<DateTimeOffset>(), Arg.Any<CancellationToken>());
    }
}