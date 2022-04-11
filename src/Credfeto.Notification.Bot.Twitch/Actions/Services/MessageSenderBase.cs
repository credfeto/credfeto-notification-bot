using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.StreamState;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public abstract class MessageSenderBase
{
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    protected MessageSenderBase(IMessageChannel<TwitchChatMessage> twitchChatMessageChannel)
    {
        this._twitchChatMessageChannel = twitchChatMessageChannel;
    }

    protected ValueTask SendMessageAsync(in Streamer streamer, string message, in CancellationToken cancellationToken)
    {
        TwitchChatMessage toSend = new(streamer: streamer, message: message);

        return this._twitchChatMessageChannel.PublishAsync(message: toSend, cancellationToken: cancellationToken);
    }
}