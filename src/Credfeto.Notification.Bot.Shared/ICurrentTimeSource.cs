using System;

namespace Credfeto.Notification.Bot.Twitch.Resources;

public interface ICurrentTimeSource
{
    DateTime UtcNow();
}