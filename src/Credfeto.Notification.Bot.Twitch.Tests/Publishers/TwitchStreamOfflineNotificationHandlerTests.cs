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
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchStreamOfflineNotificationHandlerTests : TestBase
{
    private static readonly Streamer Streamer = Streamer.FromString(nameof(Streamer));

    private readonly INotificationHandler<TwitchStreamOffline> _notificationHandler;
    private readonly ITwitchChannelManager _twitchChannelManager;
    private readonly ITwitchChannelState _twitchChannelState;

    public TwitchStreamOfflineNotificationHandlerTests()
    {
        this._twitchChannelManager = GetSubstitute<ITwitchChannelManager>();
        this._twitchChannelState = GetSubstitute<ITwitchChannelState>();

        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions { Channels = new() { new() { ChannelName = Streamer.Value } } });

        this._notificationHandler = new TwitchStreamOfflineNotificationHandler(options: options,
                                                                               twitchChannelManager: this._twitchChannelManager,
                                                                               this.GetTypedLogger<TwitchStreamOfflineNotificationHandler>());
    }

    [Fact]
    public async Task HandleAsync()
    {
        this._twitchChannelManager.GetStreamer(Streamer)
            .Returns(this._twitchChannelState);

        await this._notificationHandler.Handle(new(streamer: Streamer, title: "Skydiving", gameName: "IRL", new(year: 2020, month: 1, day: 1)),
                                               cancellationToken: CancellationToken.None);

        this._twitchChannelManager.Received(1)
            .GetStreamer(Streamer);

        this._twitchChannelState.Received(1)
            .Offline();
    }
}