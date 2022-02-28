using System;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchUser
{
    public TwitchUser(string userName, bool isBroadcaster, DateTime? dateCreated)
    {
        this.UserName = userName;
        this.IsBroadcaster = isBroadcaster;
        this.DateCreated = dateCreated;
    }

    public string UserName { get; }

    public bool IsBroadcaster { get; }

    public DateTime? DateCreated { get; }
}