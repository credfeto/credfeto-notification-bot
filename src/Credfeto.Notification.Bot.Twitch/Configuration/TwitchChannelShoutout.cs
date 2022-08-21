using System.Collections.Generic;
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

    public IReadOnlyList<TwitchFriendChannel>? FriendChannels { get; }
}