using Credfeto.Notification.Bot.Twitch.StreamState;

namespace Credfeto.Notification.Bot.Twitch;

/// <summary>
///     Twitch stream
/// </summary>
public interface ITwitchChannelManager
{
    /// <summary>
    ///     Gets the current channel.
    /// </summary>
    /// <param name="channel">The channel name.</param>
    /// <returns></returns>
    TwitchChannelState GetChannel(string channel);
}