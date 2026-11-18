using System.ComponentModel.DataAnnotations;
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

    [Required]
    public string OAuthToken { get; set; }

    [Required]
    public string UserName { get; set; }
}
