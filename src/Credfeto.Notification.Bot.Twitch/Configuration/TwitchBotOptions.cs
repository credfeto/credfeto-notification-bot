using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchBotOptions
{
    public TwitchBotOptions()
    {
        this.Authentication = new();
        this.ChatCommands = [];
    }

    [Required]
    [ValidateObjectMembers]
    public TwitchAuthentication Authentication { get; set; }

    [Required]
    [ValidateEnumeratedItems]
    public List<TwitchChatCommand> ChatCommands { get; set; }
}
