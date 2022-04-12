using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch;

/// <summary>
///     Stream Status
/// </summary>
public interface ITwitchStreamStatus
{
    /// <summary>
    ///     Enables a streamer;
    /// </summary>
    /// <param name="streamer">Streamer.</param>
    Task EnableAsync(Streamer streamer);

    /// <summary>
    ///     Updates the status.
    /// </summary>
    Task UpdateAsync();
}