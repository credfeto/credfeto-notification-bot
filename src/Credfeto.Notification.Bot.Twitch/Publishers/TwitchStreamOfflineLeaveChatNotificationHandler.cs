using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchStreamOfflineLeaveChatNotificationHandler : INotificationHandler<TwitchStreamOffline>
{
    private readonly ITwitchChat _twitchChat;

    public TwitchStreamOfflineLeaveChatNotificationHandler(ITwitchChat twitchChat)
    {
        this._twitchChat = twitchChat ?? throw new ArgumentNullException(nameof(twitchChat));
    }

    public Task Handle(TwitchStreamOffline notification, CancellationToken cancellationToken)
    {
        this._twitchChat.LeaveChat(notification.Streamer);

        return Task.CompletedTask;
    }
}