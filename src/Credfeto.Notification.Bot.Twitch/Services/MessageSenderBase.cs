using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Resources;

namespace Credfeto.Notification.Bot.Twitch.Services;

public abstract class MessageSenderBase
{
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    protected MessageSenderBase(IMessageChannel<TwitchChatMessage> twitchChatMessageChannel)
    {
        this._twitchChatMessageChannel = twitchChatMessageChannel;
    }

    protected ValueTask SendMessageAsync(string channel, string message, in CancellationToken cancellationToken)
    {
        TwitchChatMessage toSend = new(channel: channel, message: message);

        return this._twitchChatMessageChannel.PublishAsync(message: toSend, cancellationToken: cancellationToken);
    }
}