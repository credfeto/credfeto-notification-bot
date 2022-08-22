using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchBotOptions
{
    [JsonConstructor]
    public TwitchBotOptions(TwitchAuthentication authentication,
                            List<TwitchModChannel> channels,
                            List<string> heists,
                            List<string> ignoredUsers,
                            TwitchMilestones milestones,
                            List<TwitchMarbles>? marbles)
    {
        this.Authentication = authentication;
        this.Channels = channels;
        this.Heists = heists;
        this.IgnoredUsers = ignoredUsers;
        this.Milestones = milestones;
        this.Marbles = marbles;
    }

    public TwitchAuthentication Authentication { get; }

    public IReadOnlyList<TwitchModChannel> Channels { get; }

    public IReadOnlyList<string> Heists { get; }

    public IReadOnlyList<TwitchMarbles>? Marbles { get; }

    public IReadOnlyList<string> IgnoredUsers { get; }

    public TwitchMilestones Milestones { get; }
}