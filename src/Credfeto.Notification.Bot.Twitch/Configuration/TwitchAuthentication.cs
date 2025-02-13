using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchAuthentication
{
    [JsonConstructor]
    public TwitchAuthentication(TwitchAuthenticationChat chat, TwitchAuthenticationApi api)
    {
        this.Chat = chat;
        this.Api = api;
    }

    public TwitchAuthenticationApi Api { get; }

    public TwitchAuthenticationChat Chat { get; }
}
