using System.Collections.Generic;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchBotOptions
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public TwitchAuthentication Authentication { get; init; } = default!;

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public List<string> Channels { get; init; } = default!;

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public List<TwitchChannelShoutout> Shoutouts { get; init; } = default!;

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public List<string> Heists { get; init; } = default!;

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public List<string> Raids { get; init; } = default!;

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public List<string> IgnoredUsers { get; init; } = default!;
}