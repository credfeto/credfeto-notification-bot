using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchMilestones
{
    [JsonConstructor]
    [SuppressMessage("Meziantou.Analyzer", "MA0109: Use a span", Justification = "Not in this case")]
    public TwitchMilestones(int[] followers, int[] subscribers)
    {
        this.Followers = followers;
        this.Subscribers = subscribers;
    }

    public int[] Followers { get; }

    public int[] Subscribers { get; }
}