using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch;

public interface ITwitchFollowerDetector
{
    ValueTask EnableAsync(TwitchUser streamer);

    Task UpdateAsync();
}