using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;

namespace Credfeto.Notification.Bot.Twitch;

/// <summary>
///     Detectors of followers.
/// </summary>
public interface ITwitchFollowerDetector
{
    /// <summary>
    ///     Enables a streamer;
    /// </summary>
    /// <param name="streamer">Streamer.</param>
    Task EnableAsync(TwitchUser streamer);

    /// <summary>
    ///     Updates the status.
    /// </summary>
    Task UpdateAsync();
}