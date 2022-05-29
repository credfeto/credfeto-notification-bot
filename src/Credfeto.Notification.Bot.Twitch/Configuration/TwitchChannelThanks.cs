using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchChannelThanks
{
    [JsonConstructor]
    public TwitchChannelThanks(bool enabled)
    {
        this.Enabled = enabled;
    }

    public bool Enabled { get; }
}