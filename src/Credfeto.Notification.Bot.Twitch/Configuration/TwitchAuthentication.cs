using System.Diagnostics;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

[DebuggerDisplay("Api: {Api.ClientId} Chat: {Chat.UserName}")]
public sealed class TwitchAuthentication
{
    public TwitchAuthentication()
    {
        this.Api = new();
        this.Chat = new();
    }

    public TwitchAuthenticationApi Api { get; set; }

    public TwitchAuthenticationChat Chat { get; set; }
}
