using System.Diagnostics;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;

[DebuggerDisplay("{ChatUser}: {Regular}")]
public sealed class TwitchRegularChatter
{
    public TwitchRegularChatter(string chatUser, bool regular)
    {
        this.ChatUser = chatUser;
        this.Regular = regular;
    }

    public string ChatUser { get; }

    public bool Regular { get; }
}