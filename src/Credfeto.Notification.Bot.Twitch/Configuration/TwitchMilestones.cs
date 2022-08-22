using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchMilestones
{
    [JsonConstructor]
    public TwitchMilestones(int[] followers, int[] subscribers)
    {
        this.Followers = followers;
        this.Subscribers = subscribers;
    }

    public int[] Followers { get; }

    public int[] Subscribers { get; }
}