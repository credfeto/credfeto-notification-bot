using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch;

/// <summary>
///     Twitch Chat
/// </summary>
public interface ITwitchChat
{
    /// <summary>
    ///     Join the chat for the streamer.
    /// </summary>
    /// <param name="streamer">The streamer's chat to join.</param>
    void JoinChat(Streamer streamer);

    /// <summary>
    ///     Leave the chat for the streamer.
    /// </summary>
    /// <param name="streamer">The streamer's chat to leave.</param>
    void LeaveChat(Streamer streamer);

    /// <summary>
    ///     Updates the status.
    /// </summary>
    Task UpdateAsync();
}