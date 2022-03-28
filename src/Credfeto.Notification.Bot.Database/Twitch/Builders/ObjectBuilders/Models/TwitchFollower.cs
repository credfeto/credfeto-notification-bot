using System.Diagnostics;

namespace Credfeto.Notification.Bot.Database.Twitch.Builders.ObjectBuilders.Models;

[DebuggerDisplay("{Channel}:{Follower} Followed {FollowCount} times Fresh: {FreshlyReached")]
public sealed class TwitchFollower
{
    public TwitchFollower(string channel, string follower, int followCount, bool freshlyReached)
    {
        this.Channel = channel;
        this.Follower = follower;
        this.FollowCount = followCount;
        this.FreshlyReached = freshlyReached;
    }

    public string Channel { get; }

    public string Follower { get; }

    public int FollowCount { get; }

    public bool FreshlyReached { get; }
}