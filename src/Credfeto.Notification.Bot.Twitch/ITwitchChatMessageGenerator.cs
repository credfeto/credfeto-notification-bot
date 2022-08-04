using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch;

public interface ITwitchChatMessageGenerator
{
    string ThanksForBits(in Viewer user);
}