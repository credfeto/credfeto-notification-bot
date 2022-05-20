using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchChannelShoutout
{
    public bool Enabled { get; init; }

    [SuppressMessage(category: "ReSharper", checkId: "AutoPropertyCanBeMadeGetOnly.Global", Justification = "TODO: Review")]
    public List<TwitchFriendChannel>? FriendChannels { get; init; }
}