using System.Diagnostics;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.StreamState;

[DebuggerDisplay("{Streamer}: {Message}")]
public sealed class TwitchChatMessage
{
    public TwitchChatMessage(in Streamer streamer, string message)
    {
        this.Streamer = streamer;
        this.Message = message;
    }

    public Streamer Streamer { get; }

    public string Message { get; }
}