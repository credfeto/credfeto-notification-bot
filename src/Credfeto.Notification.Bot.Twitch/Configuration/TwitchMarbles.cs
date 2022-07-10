using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchMarbles
{
    [JsonConstructor]
    public TwitchMarbles(string streamer, string bot, string match)
    {
        this.Streamer = streamer;
        this.Bot = bot;
        this.Match = match;
    }

    public string Streamer { get; }

    public string Bot { get; }

    public string Match { get; }
}