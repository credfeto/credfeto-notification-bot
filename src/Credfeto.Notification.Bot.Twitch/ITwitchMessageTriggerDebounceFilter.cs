using Credfeto.Notification.Bot.Twitch.Models;

namespace Credfeto.Notification.Bot.Twitch;

public interface ITwitchMessageTriggerDebounceFilter
{
    bool CanSend(TwitchOutputMessageMatch match);
}
