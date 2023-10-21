using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchChannelRaids
{
    [JsonConstructor]
    [SuppressMessage(category: "Meziantou.Analyzer", checkId: "MA0109: Use a span", Justification = "Not in this case")]
    public TwitchChannelRaids(bool enabled, string[]? immediate, string[]? calmDown)
    {
        this.Enabled = enabled;
        this.Immediate = immediate;
        this.CalmDown = calmDown;
    }

    public bool Enabled { get; }

    public string[]? Immediate { get; }

    public string[]? CalmDown { get; }
}