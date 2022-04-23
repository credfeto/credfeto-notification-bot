using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using MediatR;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchStreamOnlineNotificationHandlerTests : TestBase
{
    private static readonly Streamer Streamer = Streamer.FromString(nameof(Streamer));

    private readonly INotificationHandler<TwitchStreamOnline> _notificationHandler;

    private readonly ITwitchChannelManager _twitchChannelManager;
    private readonly ITwitchChannelState _twitchChannelState;

    public TwitchStreamOnlineNotificationHandlerTests()
    {
        this._twitchChannelManager = GetSubstitute<ITwitchChannelManager>();
        this._twitchChannelState = GetSubstitute<ITwitchChannelState>();

        this._notificationHandler = new TwitchStreamOnlineNotificationHandler(twitchChannelManager: this._twitchChannelManager, this.GetTypedLogger<TwitchStreamOnlineNotificationHandler>());
    }

    [Fact]
    public async Task HandleAsync()
    {
        this._twitchChannelManager.GetStreamer(Streamer)
            .Returns(this._twitchChannelState);

        await this._notificationHandler.Handle(new(streamer: Streamer, title: "Skydiving", gameName: "IRL", new(year: 2020, month: 1, day: 1)), cancellationToken: CancellationToken.None);

        this._twitchChannelManager.Received(1)
            .GetStreamer(Streamer);

        await this._twitchChannelState.Received(1)
                  .OnlineAsync(gameName: "IRL", new(year: 2020, month: 1, day: 1));
    }
}