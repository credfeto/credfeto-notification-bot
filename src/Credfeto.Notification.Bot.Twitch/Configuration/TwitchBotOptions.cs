using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchBotOptions
{
    [JsonConstructor]
    public TwitchBotOptions(TwitchAuthentication authentication, List<TwitchModChannel> channels, List<string> heists, List<string> ignoredUsers, TwitchMilestones milestones)
    {
        this.Authentication = authentication;
        this.Channels = channels;
        this.Heists = heists;
        this.IgnoredUsers = ignoredUsers;
        this.Milestones = milestones;
    }

    public TwitchAuthentication Authentication { get; }

    public List<TwitchModChannel> Channels { get; }

    public List<string> Heists { get; }

    public List<string> IgnoredUsers { get; }

    public TwitchMilestones Milestones { get; }
}