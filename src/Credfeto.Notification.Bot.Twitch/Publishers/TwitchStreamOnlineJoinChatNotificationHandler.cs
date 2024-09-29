using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Date.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Mediator;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchStreamOnlineJoinChatNotificationHandler : INotificationHandler<TwitchStreamOnline>
{
    private readonly ITwitchChat _twitchChat;
    private readonly ITwitchStreamStateManager _twitchStreamStateManager;

    public TwitchStreamOnlineJoinChatNotificationHandler(ITwitchChat twitchChat, ITwitchStreamStateManager twitchStreamStateManager)
    {
        this._twitchChat = twitchChat ?? throw new ArgumentNullException(nameof(twitchChat));
        this._twitchStreamStateManager = twitchStreamStateManager;
    }

    public async ValueTask Handle(TwitchStreamOnline notification, CancellationToken cancellationToken)
    {
        await this._twitchStreamStateManager.Get(notification.Streamer)
                  .OnlineAsync(gameName: notification.GameName, notification.StartedAt.AsDateTimeOffset(), cancellationToken: cancellationToken);

        this._twitchChat.JoinChat(notification.Streamer);
    }
}