using System;
using System.Diagnostics;

namespace Credfeto.Notification.Bot.Twitch.DataTypes;

[DebuggerDisplay("{UserName}: Id: {Id} Streamer: {IsStreamer} Created: {DateCreated}")]
public sealed class TwitchUser
{
    public TwitchUser(string id, in Viewer userName, bool isStreamer, in DateTime dateCreated)
    {
        this.Id = id;
        this.UserName = userName;
        this.IsStreamer = isStreamer;
        this.DateCreated = dateCreated;
    }

    public string Id { get; }

    public Viewer UserName { get; }

    public bool IsStreamer { get; }

    public DateTime DateCreated { get; }
}