using System.Collections.Generic;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchChannelShoutout
{
    public bool Enabled { get; init; }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public List<TwitchFriendChannel>? FriendChannels { get; init; }
}