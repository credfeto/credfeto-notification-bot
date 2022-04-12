namespace Credfeto.Notification.Bot.Twitch.Configuration;

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