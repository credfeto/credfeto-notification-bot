using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Models;
using Mediator;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchStreamOnlineJoinChatNotificationHandler : INotificationHandler<TwitchStreamOnline>
{
    private readonly ITwitchChat _twitchChat;

    public TwitchStreamOnlineJoinChatNotificationHandler(ITwitchChat twitchChat)
    {
        this._twitchChat = twitchChat ?? throw new ArgumentNullException(nameof(twitchChat));
    }

    public ValueTask Handle(TwitchStreamOnline notification, CancellationToken cancellationToken)
    {
        this._twitchChat.JoinChat(notification.Streamer);

        return ValueTask.CompletedTask;
    }
}