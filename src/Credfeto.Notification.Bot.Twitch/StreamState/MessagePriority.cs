namespace Credfeto.Notification.Bot.Twitch.StreamState;

/// <summary>
///     Message priorities
/// </summary>
public enum MessagePriority
{
    /// <summary>
    ///     Deliver as soon as possible.
    /// </summary>
    ASAP = 1,

    /// <summary>
    ///     Pretend to be human.
    /// </summary>
    NATURAL = 30,

    /// <summary>
    ///     Don't care, just do as and when.
    /// </summary>
    SLOW = 120
}