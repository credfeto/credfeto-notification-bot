using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch;

/// <summary>
///     Twitch bot.
/// </summary>
public interface ITwitchBot
{
    /// <summary>
    ///     Updates the status.
    /// </summary>
    Task UpdateAsync();
}