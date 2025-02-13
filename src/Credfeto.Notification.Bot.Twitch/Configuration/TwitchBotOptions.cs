using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

public sealed class TwitchBotOptions
{
    [JsonConstructor]
    [SuppressMessage(
        category: "Meziantou.Analyzer",
        checkId: "MA0109: Use a span",
        Justification = "Not in this case"
    )]
    public TwitchBotOptions(TwitchAuthentication authentication, TwitchChatCommand[] chatCommands)
    {
        this.Authentication = authentication;
        this.ChatCommands = chatCommands;
    }

    public TwitchAuthentication Authentication { get; }

    public TwitchChatCommand[] ChatCommands { get; }
}
