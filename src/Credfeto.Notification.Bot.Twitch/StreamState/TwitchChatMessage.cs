using System.Diagnostics;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.StreamState;

[DebuggerDisplay("{Streamer}: {Priority}  {Message}")]
public sealed class TwitchChatMessage
{
    public TwitchChatMessage(in Streamer streamer, MessagePriority priority, string message)
    {
        this.Streamer = streamer;
        this.Priority = priority;
        this.Message = message;
    }

    public Streamer Streamer { get; }

    public string Message { get; }

    public MessagePriority Priority { get; }
}