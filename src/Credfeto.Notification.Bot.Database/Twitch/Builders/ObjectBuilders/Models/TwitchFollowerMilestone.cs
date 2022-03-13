using System.Diagnostics;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;

[DebuggerDisplay("{Channel}: Followers: {Followers} New {FreshlyReached}")]
public sealed class TwitchFollowerMilestone
{
    public TwitchFollowerMilestone(string channel, int followers, bool freshlyReached)
    {
        this.Channel = channel;
        this.Followers = followers;
        this.FreshlyReached = freshlyReached;
    }

    public string Channel { get; }

    public int Followers { get; }

    public bool FreshlyReached { get; }
}