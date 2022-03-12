using System;
using System.Linq;
using Credfeto.Notification.Bot.Twitch.Configuration;
using TwitchLib.Api;

namespace Credfeto.Notification.Bot.Twitch.Extensions;

internal static class OptionsExtensions
{
    public static bool IsModChannel(this TwitchBotOptions options, string channel)
    {
        return options.Channels.Any(c => StringComparer.InvariantCultureIgnoreCase.Equals(x: c, y: channel));
    }

    public static bool RaidWelcomeEnabled(this TwitchBotOptions options, string channel)
    {
        return options.IsModChannel(channel) && options.Raids.Any(c => StringComparer.InvariantCultureIgnoreCase.Equals(x: c, y: channel));
    }

    public static TwitchAPI ConfigureTwitchApi(this TwitchBotOptions options)
    {
        return new()
               {
                   Settings =
                   {
                       ClientId = options.Authentication.ClientId, Secret = options.Authentication.ClientSecret
                       /*
                       , AccessToken = options.Authentication.ClientAccessToken */
                   }
               };
    }

    public static bool IsIgnoredUser(this TwitchBotOptions options, string username)
    {
        return options.IgnoredUsers.Any(c => StringComparer.InvariantCultureIgnoreCase.Equals(x: c, y: username));
    }
}