using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;

namespace Credfeto.Notification.Bot.Twitch;

public interface ITwitchStreamStateManager
{
    ITwitchChannelState Get(Streamer streamer);
}