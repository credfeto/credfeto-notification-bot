using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchChannelMileStone
{
    [JsonConstructor]
    public TwitchChannelMileStone(bool enabled)
    {
        this.Enabled = enabled;
    }

    public bool Enabled { get; }
}