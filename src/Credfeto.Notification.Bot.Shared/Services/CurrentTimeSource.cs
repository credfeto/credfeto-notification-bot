using System;
using System.Diagnostics.CodeAnalysis;

namespace Credfeto.Notification.Bot.Twitch.Resources.Services;

public sealed class CurrentTimeSource : ICurrentTimeSource
{
    [SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0002: Call Date Time abstraction", Justification = "This is implementing the referenced mechanism")]
    public DateTime UtcNow()
    {
        return DateTime.UtcNow;
    }
}