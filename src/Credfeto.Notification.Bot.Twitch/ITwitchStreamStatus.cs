using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch;

public interface ITwitchStreamStatus
{
    ValueTask EnableAsync(Streamer streamer);

    Task UpdateAsync();
}