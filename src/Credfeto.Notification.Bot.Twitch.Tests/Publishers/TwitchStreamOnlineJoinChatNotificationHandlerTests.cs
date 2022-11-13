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

    public TwitchStreamOnlineJoinChatNotificationHandlerTests()
    {
        this._twitchChat = GetSubstitute<ITwitchChat>();

        this._notificationHandler = new TwitchStreamOnlineJoinChatNotificationHandler(this._twitchChat);
    }

    [Fact]
    public async Task HandleShouldLeaveChatAsync()
    {
        TwitchStreamOnline notification = new(streamer: MockReferenceData.Streamer, title: "Banana", gameName: "GameName", startedAt: DateTime.MinValue);

        await this._notificationHandler.Handle(notification: notification, cancellationToken: CancellationToken.None);

        this._twitchChat.Received(1)
            .JoinChat(streamer: MockReferenceData.Streamer);
    }
}