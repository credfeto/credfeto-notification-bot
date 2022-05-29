using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchChannelWelcome
{
    [JsonConstructor]
    public TwitchChannelWelcome(bool enabled)
    {
        this.Enabled = enabled;
    }

    public bool Enabled { get; }
}