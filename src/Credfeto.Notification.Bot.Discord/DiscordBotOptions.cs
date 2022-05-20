using System.Diagnostics.CodeAnalysis;

namespace Credfeto.Notification.Bot.Discord;

public sealed class DiscordBotOptions
{
    [SuppressMessage(category: "ReSharper", checkId: "AutoPropertyCanBeMadeGetOnly.Global", Justification = "TODO: Review")]
    public string Token { get; init; } = default!;
}