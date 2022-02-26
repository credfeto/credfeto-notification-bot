using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch;

/// <summary>
///     Twitch Chat
/// </summary>
public interface ITwitchChat
{
    /// <summary>
    ///     Updates the status.
    /// </summary>
    Task UpdateAsync();
}