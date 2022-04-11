using Credfeto.Notification.Bot.Twitch.DataTypes;
using MediatR;

namespace Credfeto.Notification.Bot.Twitch.Models;

public sealed class TwitchFollowerMilestoneReached : INotification
{
    public TwitchFollowerMilestoneReached(in Channel channel, int milestoneReached, int nextMilestone, double progress)
    {
        this.Channel = channel;
        this.MilestoneReached = milestoneReached;
        this.NextMilestone = nextMilestone;
        this.Progress = progress;
    }

    public Channel Channel { get; }

    public int MilestoneReached { get; }

    public int NextMilestone { get; }

    public double Progress { get; }
}