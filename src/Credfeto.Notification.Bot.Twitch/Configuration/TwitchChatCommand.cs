using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchChatCommand
{
    [JsonConstructor]
    public TwitchChatCommand(
        string streamer,
        string bot,
        string match,
        string issue,
        string matchType
    )
    {
        this.Streamer = streamer;
        this.Bot = bot;
        this.Match = match;
        this.Issue = issue;
        this.MatchType = matchType;
    }

    public string Streamer { get; }

    public string Bot { get; }

    public string Match { get; }

    public string Issue { get; }

    public string MatchType { get; }
}
