using System.Diagnostics.CodeAnalysis;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchModChannel
{
    [SuppressMessage(category: "ReSharper", checkId: "AutoPropertyCanBeMadeGetOnly.Global", Justification = "TODO: Review")]
    public string ChannelName { get; init; } = default!;

    public TwitchChannelShoutout ShoutOuts { get; init; } = default!;

    public TwitchChannelRaids Raids { get; init; } = default!;

    public TwitchChannelThanks Thanks { get; init; } = default!;

    public TwitchChannelMileStone MileStones { get; init; } = default!;

    public TwitchChannelWelcome Welcome { get; init; } = default!;
}