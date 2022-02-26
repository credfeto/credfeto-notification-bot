using Credfeto.Notification.Bot.Twitch.Services;

namespace Credfeto.Notification.Bot.Twitch;

/// <summary>
///     Twitch stream
/// </summary>
public interface ITwitchChannelManager
{
    /// <summary>
    ///     Gets the current channel.
    /// </summary>
    /// <param name="channel"></param>
    /// <returns></returns>
    ChannelState GetChannel(string channel);
}