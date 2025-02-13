using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchAuthenticationApi
{
    [JsonConstructor]
    public TwitchAuthenticationApi(string clientId, string clientSecret, string clientAccessToken)
    {
        this.ClientId = clientId;
        this.ClientSecret = clientSecret;
        this.ClientAccessToken = clientAccessToken;
    }

    public string ClientId { get; }

    public string ClientSecret { get; }

    public string ClientAccessToken { get; }
}
