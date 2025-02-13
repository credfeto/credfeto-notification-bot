using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using Mediator;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchStreamOnlineJoinChatNotificationHandlerTests : TestBase
{
    private readonly INotificationHandler<TwitchStreamOnline> _notificationHandler;
    private readonly ITwitchChat _twitchChat;
    private readonly ITwitchStreamStateManager _twitchStreamStateManager;

    public TwitchStreamOnlineJoinChatNotificationHandlerTests()
    {
        this._twitchChat = GetSubstitute<ITwitchChat>();
        this._twitchStreamStateManager = GetSubstitute<ITwitchStreamStateManager>();

        this._notificationHandler = new TwitchStreamOnlineJoinChatNotificationHandler(
            twitchChat: this._twitchChat,
            twitchStreamStateManager: this._twitchStreamStateManager
        );
    }

    [Fact]
    public async Task HandleShouldJoinChatAsync()
    {
        TwitchStreamOnline notification = new(
            streamer: MockReferenceData.Streamer,
            title: "Banana",
            gameName: "GameName",
            new(year: 2024, month: 1, day: 1, hour: 1, minute: 1, second: 1, kind: DateTimeKind.Utc)
        );

        await this._notificationHandler.Handle(
            notification: notification,
            cancellationToken: CancellationToken.None
        );

        await this
            ._twitchStreamStateManager.Received(1)
            .Get(streamer: MockReferenceData.Streamer)
            .OnlineAsync(
                Arg.Any<string>(),
                Arg.Any<DateTimeOffset>(),
                Arg.Any<CancellationToken>()
            );

        this._twitchChat.Received(1).JoinChat(streamer: MockReferenceData.Streamer);
    }
}
