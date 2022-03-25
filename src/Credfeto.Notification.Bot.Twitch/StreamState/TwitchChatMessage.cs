using System.Diagnostics;

namespace Credfeto.Notification.Bot.Twitch.StreamState;

[DebuggerDisplay("{Channel}: {Message}")]
public sealed class TwitchChatMessage
{
    public TwitchChatMessage(string channel, string message)
    {
        this.Channel = channel;
        this.Message = message;
    }

    public string Channel { get; }

    public string Message { get; }
}