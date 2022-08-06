using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchAuthentication
{
    [JsonConstructor]
    public TwitchAuthentication(string oAuthToken, string userName, string clientId, string clientSecret, string clientAccessToken)
    {
        // Chat
        this.OAuthToken = oAuthToken;
        this.UserName = userName;

        // API
        this.ClientId = clientId;
        this.ClientSecret = clientSecret;
        this.ClientAccessToken = clientAccessToken;
    }

    public string OAuthToken { get; }

    public string UserName { get; }

    public string ClientId { get; }

    public string ClientSecret { get; }

    public string ClientAccessToken { get; }
}