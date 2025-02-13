using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch;

public interface ITwitchChat
{
    void JoinChat(Streamer streamer);

    void LeaveChat(Streamer streamer);

    Task UpdateAsync();
}
