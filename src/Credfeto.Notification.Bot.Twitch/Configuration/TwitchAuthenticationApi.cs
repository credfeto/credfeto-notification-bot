using System.Diagnostics;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

[DebuggerDisplay("ClientId: {ClientId}")]
public sealed class TwitchAuthenticationApi
{
    public TwitchAuthenticationApi()
    {
        this.ClientId = "n/a";
        this.ClientSecret = "n/a";
        this.ClientAccessToken = "n/a";
    }

    public string ClientId { get; set; }

    public string ClientSecret { get; set; }

    public string ClientAccessToken { get; set; }
}
