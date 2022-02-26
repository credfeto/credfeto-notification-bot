using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch;

/// <summary>
///     Stream Status
/// </summary>
public interface ITwitchStreamStatus
{
    /// <summary>
    ///     Updates the status.
    /// </summary>
    Task UpdateAsync();
}