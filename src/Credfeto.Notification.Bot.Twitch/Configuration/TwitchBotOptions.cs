using System.Collections.Generic;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchBotOptions
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public TwitchAuthentication Authentication { get; init; } = default!;

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public List<TwitchModChannel> Channels { get; init; } = default!;

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public List<string> Heists { get; init; } = default!;

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public List<string> IgnoredUsers { get; init; } = default!;

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public TwitchMilestones Milestones { get; init; } = default!;
}

public sealed class TwitchModChannel
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public string ChannelName { get; init; } = default!;

    public TwitchChannelShoutout ShoutOuts { get; init; } = default!;

    public TwitchChannelRaids Raids { get; init; } = default!;

    public TwitchChannelThanks Thanks { get; init; } = default!;

    public TwitchChannelMileStone MileStones { get; init; } = default!;

    public TwitchChannelWelcome Welcome { get; init; } = default!;
}