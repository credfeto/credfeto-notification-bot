using System;
using Discord;

namespace Credfeto.Notification.Bot.Discord.Models;

public sealed class DiscordMessage
{
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