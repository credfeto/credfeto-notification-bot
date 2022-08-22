using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchChannelShoutout
{
    [JsonConstructor]
    public TwitchChannelShoutout(bool enabled, List<TwitchFriendChannel>? friendChannels)
    {
        this.Enabled = enabled;
        this.FriendChannels = friendChannels;
    }

    public bool Enabled { get; }

    [SuppressMessage(category: "Meziantou.Analyzer", checkId: "MA0016: Prefer returning collection abstraction", Justification = "For serialisation")]
    public List<TwitchFriendChannel>? FriendChannels { get; }
}