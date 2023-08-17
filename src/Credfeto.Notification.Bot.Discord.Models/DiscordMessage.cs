using System;
using System.Diagnostics.CodeAnalysis;
using Discord;

namespace Credfeto.Notification.Bot.Discord.Models;

public sealed class DiscordMessage
{
    [SuppressMessage("Meziantou.Analyzer", "MA0109: Use a span", Justification = "Not in this case")]
    public DiscordMessage(string channel, Embed embed, string title, byte[]? image)
    {
        this.Channel = channel;
        this.Embed = embed ?? throw new ArgumentNullException(nameof(embed));
        this.Title = title;
        this.Image = image;
    }

    public string Channel { get; }

    public Embed Embed { get; }

    public string Title { get; }

    public byte[]? Image { get; }
}