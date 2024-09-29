using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchAuthenticationChat
{
    [JsonConstructor]
    public TwitchAuthenticationChat(string oAuthToken, string userName)
    {
        this.OAuthToken = oAuthToken;
        this.UserName = userName;
    }

    public string OAuthToken { get; }

    public string UserName { get; }
}