using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchMarbles
{
    [JsonConstructor]
    public TwitchMarbles(string streamer, string bot, string message)
    {
        this.Streamer = streamer;
        this.Bot = bot;
        this.Message = message;
    }

    public string Streamer { get; }

    public string Bot { get; }

    public string Message { get; }
}