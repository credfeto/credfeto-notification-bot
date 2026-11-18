using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchChatCommand
{
    [JsonConstructor]
    public TwitchChatCommand(string streamer, string bot, string match, string issue, string matchType)
    {
        this.Streamer = streamer;
        this.Bot = bot;
        this.Match = match;
        this.Issue = issue;
        this.MatchType = matchType;
    }

    [Required]
    public string Streamer { get; }

    [Required]
    public string Bot { get; }

    [Required]
    public string Match { get; }

    [Required]
    public string Issue { get; }

    public string MatchType { get; }
}
