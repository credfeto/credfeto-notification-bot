using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using MediatR;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchStreamOfflineAutoHostHandlerTests : TestBase
{
    private static readonly Streamer Streamer = Streamer.FromString(nameof(Streamer));

    private readonly IHoster _hoster;

    private readonly INotificationHandler<TwitchStreamOffline> _notificationHandler;

    public TwitchStreamOfflineAutoHostHandlerTests()
    {
        this._hoster = GetSubstitute<IHoster>();
        this._notificationHandler = new TwitchStreamOfflineAutoHostHandler(this._hoster);
    }

    [Fact]
    public async Task HandleShouldStopHostingAsync()
    {
        TwitchStreamOffline notification = new(streamer: Streamer, title: "Banana", gameName: "GameName", startedAt: DateTime.MinValue);

        await this._notificationHandler.Handle(notification: notification, cancellationToken: CancellationToken.None);

        await this._hoster.Received(1)
                  .StreamOfflineAsync(streamer: Streamer, Arg.Any<CancellationToken>());
    }
}