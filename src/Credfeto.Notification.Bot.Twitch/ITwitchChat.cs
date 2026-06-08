using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch;

public interface ITwitchChat
{
    Task JoinChatAsync(Streamer streamer);

    Task LeaveChatAsync(Streamer streamer);

    Task UpdateAsync();
}
