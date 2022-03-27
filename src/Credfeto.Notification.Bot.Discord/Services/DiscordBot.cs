using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Discord.Models;
using Credfeto.Notification.Bot.Shared;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Discord.Services;

/// <summary>
///     Discord bot
/// </summary>
public sealed class DiscordBot : IDiscordBot
{
    private readonly DiscordSocketClient _client;
    private readonly ILogger<DiscordBot> _logger;
    private readonly IMessageChannel<DiscordMessage> _messageChannel;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="discordSocketClient">Discord Socket client.</param>
    /// <param name="messageChannel"></param>
    /// <param name="logger">Logging.</param>
    public DiscordBot(DiscordSocketClient discordSocketClient, IMessageChannel<DiscordMessage> messageChannel, ILogger<DiscordBot> logger)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._client = discordSocketClient ?? throw new ArgumentNullException(nameof(discordSocketClient));
        this._messageChannel = messageChannel ?? throw new ArgumentNullException(nameof(messageChannel));

        this._messageChannel.ReadAllAsync(CancellationToken.None)
            .ToObservable()
            .Select(message => Observable.FromAsync(() => this.PublishMessageAsync(message: message)))
            .Concat()
            .Subscribe();
    }

    /// <inheritdoc />
    public async Task PublishAsync(DiscordMessage message, CancellationToken cancellationToken)
    {
        await this._messageChannel.PublishAsync(message: message, cancellationToken: cancellationToken);
        this._logger.LogDebug($"{message.Channel}: Queuing message for Discord");
    }

    private async Task PublishMessageAsync(DiscordMessage message)
    {
        SocketTextChannel? socketTextChannel = this.GetChannel(message.Channel);

        if (socketTextChannel == null)
        {
            return;
        }

        try
        {
            using (socketTextChannel.EnterTypingState())
            {
                if (message.Image != null)

                {
                    await using (MemoryStream imageStream = new(buffer: message.Image, writable: false))
                    {
                        await socketTextChannel.SendFileAsync(stream: imageStream, filename: "image.png", text: message.Title, embed: message.Embed);
                    }
                }
                else
                {
                    await socketTextChannel.SendMessageAsync(text: message.Title, embed: message.Embed);
                }
            }
        }
        catch (Exception exception)
        {
            this._logger.LogCritical(new(exception.HResult), exception: exception, message: exception.Message);
        }
    }

    private SocketTextChannel? GetChannel(string channelName)
    {
        return this._client.Guilds.Select(guild => guild.TextChannels.FirstOrDefault(predicate: c => StringComparer.InvariantCultureIgnoreCase.Equals(x: c.Name, y: channelName)))
                   .FirstOrDefault(channel => channel != null);
    }
}