using System;
using System.Linq;
using Credfeto.Notification.Bot.Twitch.Configuration;
using TwitchLib.Api;

namespace Credfeto.Notification.Bot.Twitch.Extensions;

internal static class OptionsExtensions
{
    public static TwitchModChannel? GetModChannel(this TwitchBotOptions options, string channel)
    {
        return options.Channels.Find(c => StringComparer.InvariantCultureIgnoreCase.Equals(x: c.ChannelName, y: channel));
    }

    public static bool IsModChannel(this TwitchBotOptions options, string channel)
    {
        return options.GetModChannel(channel) != null;
    }

    public static bool RaidWelcomeEnabled(this TwitchBotOptions options, string channel)
    {
        return options.GetModChannel(channel)
                      ?.Raids.Enabled ?? false;
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

    public static bool IsSelf(this TwitchBotOptions options, string username)
    {
        return StringComparer.InvariantCultureIgnoreCase.Equals(x: options.Authentication.UserName, y: username);
    }
}