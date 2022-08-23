using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchBotOptions
{
    [JsonConstructor]
    public TwitchBotOptions(TwitchAuthentication authentication,
                            TwitchModChannel[] channels,
                            string[] heists,
                            string[] ignoredUsers,
                            TwitchMilestones milestones,
                            TwitchMarbles[] marbles)
    {
        this.Authentication = authentication;
        this.Channels = channels;
        this.Heists = heists;
        this.IgnoredUsers = ignoredUsers;
        this.Milestones = milestones;
        this.Marbles = marbles;
    }

    public TwitchAuthentication Authentication { get; }

    public TwitchModChannel[] Channels { get; }

    public string[] Heists { get; }

    public TwitchMarbles[] Marbles { get; }

    public string[] IgnoredUsers { get; }

    public TwitchMilestones Milestones { get; }
}