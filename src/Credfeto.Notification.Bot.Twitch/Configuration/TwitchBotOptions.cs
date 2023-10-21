using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchBotOptions
{
    [JsonConstructor]
    [SuppressMessage(category: "Meziantou.Analyzer", checkId: "MA0109: Use a span", Justification = "Not in this case")]
    public TwitchBotOptions(TwitchAuthentication authentication, TwitchModChannel[] channels, string[] ignoredUsers, TwitchMilestones milestones, TwitchChatCommand[] chatCommands)
    {
        this.Authentication = authentication;
        this.Channels = channels;
        this.IgnoredUsers = ignoredUsers;
        this.Milestones = milestones;
        this.ChatCommands = chatCommands;
    }

    public TwitchAuthentication Authentication { get; }

    public TwitchModChannel[] Channels { get; }

    public TwitchChatCommand[] ChatCommands { get; }

    public string[] IgnoredUsers { get; }

    public TwitchMilestones Milestones { get; }
}