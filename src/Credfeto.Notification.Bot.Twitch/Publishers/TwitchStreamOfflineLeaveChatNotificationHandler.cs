using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Models;
using Mediator;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchStreamOfflineLeaveChatNotificationHandler : INotificationHandler<TwitchStreamOffline>
{
    private readonly ITwitchChat _twitchChat;
    private readonly ITwitchStreamStateManager _twitchStreamStateManager;

    public TwitchStreamOfflineLeaveChatNotificationHandler(
        ITwitchChat twitchChat,
        ITwitchStreamStateManager twitchStreamStateManager
    )
    {
        this._twitchChat = twitchChat ?? throw new ArgumentNullException(nameof(twitchChat));
        this._twitchStreamStateManager = twitchStreamStateManager;
    }

    public ValueTask Handle(TwitchStreamOffline notification, CancellationToken cancellationToken)
    {
        this._twitchStreamStateManager.Get(notification.Streamer).Offline();
        this._twitchChat.LeaveChat(notification.Streamer);

        return ValueTask.CompletedTask;
    }
}
