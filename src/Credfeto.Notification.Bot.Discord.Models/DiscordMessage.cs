using System;
using Discord;

namespace Credfeto.Notification.Bot.Discord.Models;

/// <summary>
///     A message to publish
/// </summary>
public sealed class DiscordMessage
{
    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="channel">The channel to publish the message in.</param>
    /// <param name="embed">The message embed.</param>
    /// <param name="title">The title.</param>
    /// <param name="image">An optional image to publish with the message.</param>
    public DiscordMessage(string channel, Embed embed, string title, byte[]? image)
    {
        this.Channel = channel;
        this.Embed = embed ?? throw new ArgumentNullException(nameof(embed));
        this.Title = title;
        this.Image = image;
    }

    /// <summary>
    ///     The channel to publish the message in.
    /// </summary>
    public string Channel { get; }

    /// <summary>
    ///     The message embed.
    /// </summary>
    public Embed Embed { get; }

    /// <summary>
    ///     The Title.
    /// </summary>
    public string Title { get; }

    /// <summary>
    ///     An optional image to publish with the message.
    /// </summary>
    public byte[]? Image { get; }
}