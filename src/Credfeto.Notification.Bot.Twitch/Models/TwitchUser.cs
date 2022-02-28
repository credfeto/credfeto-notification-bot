using System;
using System.Diagnostics;

namespace Credfeto.Notification.Bot.Twitch.Models;

[DebuggerDisplay("{UserName}: Streamer: {IsStreamer} Created: {DateCreated}")]
public sealed class TwitchUser
{
    public TwitchUser(string userName, bool isStreamer, DateTime? dateCreated)
    {
        this.UserName = userName;
        this.IsStreamer = isStreamer;
        this.DateCreated = dateCreated;
    }

    public string UserName { get; }

    public bool IsStreamer { get; }

    public DateTime? DateCreated { get; }
}