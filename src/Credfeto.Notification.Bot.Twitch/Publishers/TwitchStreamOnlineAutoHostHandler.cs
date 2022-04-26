using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchStreamOnlineAutoHostHandler : INotificationHandler<TwitchStreamOnline>
{
    private readonly IHoster _twitchChat;

    public TwitchStreamOnlineAutoHostHandler(IHoster twitchChat)
    {
        this._twitchChat = twitchChat ?? throw new ArgumentNullException(nameof(twitchChat));
    }

    public Task Handle(TwitchStreamOnline notification, CancellationToken cancellationToken)
    {
        return this._twitchChat.StreamOnlineAsync(streamer: notification.Streamer, streamStartTime: notification.StartedAt, cancellationToken: cancellationToken);
    }
}