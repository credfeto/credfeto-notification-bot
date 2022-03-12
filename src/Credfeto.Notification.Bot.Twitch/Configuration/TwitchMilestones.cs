using System.Collections.Generic;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchMilestones
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public List<int> Followers { get; init; } = default!;

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public List<int> Subscribers { get; init; } = default!;
}