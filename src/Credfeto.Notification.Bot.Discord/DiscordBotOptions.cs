using System.Diagnostics;
using System.Text.Json.Serialization;

namespace Credfeto.Notification.Bot.Discord;

[DebuggerDisplay("{Token}")]
public sealed class DiscordBotOptions
{
    [JsonConstructor]
    public DiscordBotOptions(string token)
    {
        this.Token = token;
    }

    public string Token { get; }
}