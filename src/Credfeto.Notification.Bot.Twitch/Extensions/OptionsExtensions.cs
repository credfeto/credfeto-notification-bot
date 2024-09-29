using System;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using TwitchLib.Api;

namespace Credfeto.Notification.Bot.Twitch.Extensions;

internal static class OptionsExtensions
{
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

    public static bool IsSelf(this TwitchBotOptions options, in Viewer username)
    {
        return StringComparer.InvariantCultureIgnoreCase.Equals(x: options.Authentication.UserName, y: username.Value);
    }
}