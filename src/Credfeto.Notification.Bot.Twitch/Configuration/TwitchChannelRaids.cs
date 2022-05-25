namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchChannelRaids
{
    public bool Enabled { get; init; }

    /// <summary>
    ///     Things to send immediately after a raider has been detected.
    /// </summary>
    public string[]? Immediate { get; init; }

    /// <summary>
    ///     Things to send once incoming raid has settled down.
    /// </summary>
    public string[]? CalmDown { get; init; }
}