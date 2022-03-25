using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using MediatR;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchStreamOfflineNotificationHandlerTests : TestBase
{
    private const string CHANNEL = nameof(CHANNEL);

    private readonly INotificationHandler<TwitchStreamOffline> _notificationHandler;
    private readonly ITwitchChannelManager _twitchChannelManager;
    private readonly ITwitchChannelState _twitchChannelState;

    public TwitchStreamOfflineNotificationHandlerTests()
    {
        this._twitchChannelManager = GetSubstitute<ITwitchChannelManager>();
        this._twitchChannelState = GetSubstitute<ITwitchChannelState>();

        this._notificationHandler = new TwitchStreamOfflineNotificationHandler(twitchChannelManager: this._twitchChannelManager, this.GetTypedLogger<TwitchStreamOfflineNotificationHandler>());
    }

    [Fact]
    public async Task HandleAsync()
    {
        this._twitchChannelManager.GetChannel(CHANNEL)
            .Returns(this._twitchChannelState);

        await this._notificationHandler.Handle(new(channel: CHANNEL, title: "Skydiving", gameName: "IRL", new(year: 2020, month: 1, day: 1)), cancellationToken: CancellationToken.None);

        this._twitchChannelManager.Received(1)
            .GetChannel(CHANNEL);

        this._twitchChannelState.Received(1)
            .Offline();
    }
}