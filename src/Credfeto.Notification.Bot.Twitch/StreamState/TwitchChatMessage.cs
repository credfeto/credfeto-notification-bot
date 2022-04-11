using System.Diagnostics;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.StreamState;

[DebuggerDisplay("{Channel}: {Message}")]
public sealed class TwitchChatMessage
{
    public TwitchChatMessage(in Channel channel, string message)
    {
        this.Channel = channel;
        this.Message = message;
    }

    public Channel Channel { get; }

    public string Message { get; }
}