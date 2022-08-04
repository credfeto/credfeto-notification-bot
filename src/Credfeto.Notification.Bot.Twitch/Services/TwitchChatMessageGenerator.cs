using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchChatMessageGenerator : ITwitchChatMessageGenerator
{
    public string ThanksForBits(in Viewer user)
    {
        return $"Thanks @{user} for the bits";
    }
}