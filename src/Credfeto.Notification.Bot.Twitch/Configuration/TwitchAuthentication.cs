using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

[DebuggerDisplay("Api: {Api.ClientId} Chat: {Chat.UserName}")]
public sealed class TwitchAuthentication
{
    public TwitchAuthentication()
    {
        this.Api = new();
        this.Chat = new();
    }

    [Required]
    [ValidateObjectMembers]
    public TwitchAuthenticationApi Api { get; set; }

    [Required]
    [ValidateObjectMembers]
    public TwitchAuthenticationChat Chat { get; set; }
}
