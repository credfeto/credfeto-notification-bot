using Credfeto.Notification.Bot.Twitch.Configuration;
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
                       ClientId = options.Authentication.Api.ClientId, Secret = options.Authentication.Api.ClientSecret
                       /*
                       , AccessToken = options.Authentication.ClientAccessToken */
                   }
               };
    }
}