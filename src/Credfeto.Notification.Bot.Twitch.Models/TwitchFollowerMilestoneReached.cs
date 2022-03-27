using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchFollowerMilestoneReached : INotification
{
    public TwitchFollowerMilestoneReached(string channel, int milestoneReached, int nextMilestone, double progress)
    {
        this.Channel = channel;
        this.MilestoneReached = milestoneReached;
        this.NextMilestone = nextMilestone;
        this.Progress = progress;
    }

    public string Channel { get; }

    public int MilestoneReached { get; }

    public int NextMilestone { get; }

    public double Progress { get; }
}