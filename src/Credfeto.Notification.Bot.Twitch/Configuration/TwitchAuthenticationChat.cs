using System.Diagnostics;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

[DebuggerDisplay("Username: {UserName}")]
public sealed class TwitchAuthenticationChat
{
    public TwitchAuthenticationChat()
    {
        this.OAuthToken = "n/a";
        this.UserName = "n/a";
    }

    public string OAuthToken { get; set; }

    public string UserName { get; set; }
}