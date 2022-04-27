using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Publishers;

public sealed class TwitchStreamOfflineAutoHostHandler : INotificationHandler<TwitchStreamOffline>
{
    private readonly IHoster _twitchChat;

    public TwitchStreamOfflineAutoHostHandler(IHoster twitchChat)
    {
        this._twitchChat = twitchChat ?? throw new ArgumentNullException(nameof(twitchChat));
    }

    public Task Handle(TwitchStreamOffline notification, CancellationToken cancellationToken)
    {
        return this._twitchChat.StreamOfflineAsync(streamer: notification.Streamer, cancellationToken: cancellationToken);
    }
}