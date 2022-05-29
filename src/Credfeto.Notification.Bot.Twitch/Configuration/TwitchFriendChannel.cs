using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchFriendChannel
{
    [JsonConstructor]
    public TwitchFriendChannel(string channel, string? message)
    {
        this.Channel = channel;
        this.Message = message;
    }

    public string Channel { get; }

    public string? Message { get; }
}