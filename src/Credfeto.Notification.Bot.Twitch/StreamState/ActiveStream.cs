using System;
using System.Diagnostics;

namespace Credfeto.Notification.Bot.Twitch.StreamState;

[DebuggerDisplay("{GameName}: Started: {StartedAt}")]
internal sealed class ActiveStream
{
    public ActiveStream(string gameName, in DateTimeOffset startedAt)
    {
        this.GameName = gameName;
        this.StartedAt = startedAt;
    }

    public string GameName { get; }

    public DateTimeOffset StartedAt { get; }

    public bool UserChatted { get; set; }
}