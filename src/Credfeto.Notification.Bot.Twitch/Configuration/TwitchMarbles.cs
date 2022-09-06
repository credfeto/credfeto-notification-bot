using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchMarbles
{
    [JsonConstructor]
    public TwitchMarbles(string streamer, string bot, string match, string issue)
    {
        this.Streamer = streamer;
        this.Bot = bot;
        this.Match = match;
        this.Issue = issue;
    }

    public string Streamer { get; }

    public string Bot { get; }

    public string Match { get; }

    public string Issue { get; }
}