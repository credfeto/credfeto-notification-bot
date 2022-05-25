using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using MediatR;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchStreamOnlineNotificationHandlerTests : TestBase
{
    private static readonly Streamer Streamer = Streamer.FromString(nameof(Streamer));
    private static readonly Streamer NonStreamer = Streamer.FromString(nameof(NonStreamer));

    private readonly INotificationHandler<TwitchStreamOnline> _notificationHandler;

    private readonly ITwitchChannelManager _twitchChannelManager;
    private readonly ITwitchChannelState _twitchChannelState;

    public TwitchStreamOnlineNotificationHandlerTests()
    {
        this._twitchChannelManager = GetSubstitute<ITwitchChannelManager>();
        this._twitchChannelState = GetSubstitute<ITwitchChannelState>();

        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions { Channels = new() { new() { ChannelName = Streamer.Value } } });

        this._notificationHandler = new TwitchStreamOnlineNotificationHandler(options: options,
                                                                              twitchChannelManager: this._twitchChannelManager,
                                                                              this.GetTypedLogger<TwitchStreamOnlineNotificationHandler>());
    }

    [Fact]
    public async Task HandleWhenModChannelAsync()
    {
        this._twitchChannelManager.GetStreamer(Streamer)
            .Returns(this._twitchChannelState);

        await this._notificationHandler.Handle(new(streamer: Streamer, title: "Skydiving", gameName: "IRL", new(year: 2020, month: 1, day: 1)), cancellationToken: CancellationToken.None);

        this._twitchChannelManager.Received(1)
            .GetStreamer(Streamer);

        await this._twitchChannelState.Received(1)
                  .OnlineAsync(gameName: "IRL", new(year: 2020, month: 1, day: 1));
    }

    [Fact]
    public async Task HandleWhenModChannelExceptionAsync()
    {
        this._twitchChannelManager.GetStreamer(Streamer)
            .Returns(this._twitchChannelState);

        this._twitchChannelState.OnlineAsync(Arg.Any<string>(), Arg.Any<DateTime>())
            .Throws<TimeoutException>();

        await this._notificationHandler.Handle(new(streamer: Streamer, title: "Skydiving", gameName: "IRL", new(year: 2020, month: 1, day: 1)), cancellationToken: CancellationToken.None);

        this._twitchChannelManager.Received(1)
            .GetStreamer(Streamer);

        await this._twitchChannelState.Received(1)
                  .OnlineAsync(gameName: "IRL", new(year: 2020, month: 1, day: 1));
    }

    [Fact]
    public async Task HandleWhenNotModChannelAsync()
    {
        this._twitchChannelManager.GetStreamer(Arg.Any<Streamer>())
            .Returns(this._twitchChannelState);

        await this._notificationHandler.Handle(new(streamer: NonStreamer, title: "Skydiving", gameName: "IRL", new(year: 2020, month: 1, day: 1)), cancellationToken: CancellationToken.None);

        this._twitchChannelManager.DidNotReceive()
            .GetStreamer(Arg.Any<Streamer>());

        await this._twitchChannelState.DidNotReceive()
                  .OnlineAsync(Arg.Any<string>(), Arg.Any<DateTime>());
    }
}