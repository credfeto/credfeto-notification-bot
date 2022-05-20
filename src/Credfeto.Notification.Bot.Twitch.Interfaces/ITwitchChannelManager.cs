using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Interfaces;

public interface ITwitchChannelManager
{
    ITwitchChannelState GetStreamer(Streamer streamer);
}