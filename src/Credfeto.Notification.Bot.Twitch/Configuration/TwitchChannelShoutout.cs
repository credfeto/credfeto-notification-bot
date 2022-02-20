using System.Collections.Generic;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchChannelShoutout
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public string Channel { get; init; } = default!;

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public List<TwitchFriendChannel> FriendChannels { get; init; } = default!;
}