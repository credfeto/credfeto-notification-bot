namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchChannelRaids
{
    public bool Enabled { get; init; }

    public string[]? Immediate { get; init; }

    public string[]? CalmDown { get; init; }
}