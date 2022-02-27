using System;

namespace Credfeto.Notification.Bot.Shared;

public interface ICurrentTimeSource
{
    DateTime UtcNow();
}