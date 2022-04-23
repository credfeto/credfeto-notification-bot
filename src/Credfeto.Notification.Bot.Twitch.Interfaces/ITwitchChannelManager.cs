using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Interfaces;

/// <summary>
///     Twitch stream
/// </summary>
public interface ITwitchChannelManager
{
    /// <summary>
    ///     Gets the current channel.
    /// </summary>
    /// <param name="streamer">The channel name.</param>
    /// <returns></returns>
    ITwitchChannelState GetStreamer(Streamer streamer);
}