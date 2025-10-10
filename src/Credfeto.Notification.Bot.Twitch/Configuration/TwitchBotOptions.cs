using System.Collections.Generic;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchBotOptions
{
    public TwitchBotOptions()
    {
        this.Authentication = new();
        this.ChatCommands = [];
    }

    public TwitchAuthentication Authentication { get; set; }

    public List<TwitchChatCommand> ChatCommands { get; set; }
}
