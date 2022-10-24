namespace Credfeto.Notification.Bot.Twitch.Models;

public enum TwitchMessageMatchType
{
    EXACT = 0,

    STARTS_WITH = 1,

    ENDS_WITH = 2,

    CONTAINS = 3,

    REGEX = 4
}