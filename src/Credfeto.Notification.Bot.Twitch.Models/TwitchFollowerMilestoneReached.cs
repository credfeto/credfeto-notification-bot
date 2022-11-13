using Credfeto.Notification.Bot.Twitch.DataTypes;
using Mediator;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchFollowerMilestoneReached : INotification
{
    public TwitchFollowerMilestoneReached(in Streamer streamer, int milestoneReached, int nextMilestone, double progress)
    {
        this.Streamer = streamer;
        this.MilestoneReached = milestoneReached;
        this.NextMilestone = nextMilestone;
        this.Progress = progress;
    }

    public Streamer Streamer { get; }

    public int MilestoneReached { get; }

    public int NextMilestone { get; }

    public double Progress { get; }
}