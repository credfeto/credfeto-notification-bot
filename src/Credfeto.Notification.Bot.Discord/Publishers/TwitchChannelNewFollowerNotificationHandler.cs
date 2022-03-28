using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Discord.Models;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Models;
using Discord;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Discord.Publishers;

public sealed class TwitchChannelNewFollowerNotificationHandler : INotificationHandler<TwitchChannelNewFollower>
{
    private readonly ILogger<TwitchChannelNewFollowerNotificationHandler> _logger;
    private readonly IMessageChannel<DiscordMessage> _messageChannel;

    public TwitchChannelNewFollowerNotificationHandler(IMessageChannel<DiscordMessage> messageChannel, ILogger<TwitchChannelNewFollowerNotificationHandler> logger)
    {
        this._messageChannel = messageChannel ?? throw new ArgumentNullException(nameof(messageChannel));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(TwitchChannelNewFollower notification, CancellationToken cancellationToken)
    {
        Embed embed = new EmbedBuilder().WithColor(Color.Blue)
                                        .WithCurrentTimestamp()
                                        .WithUrl($"https://twitch.tv/{notification.User}")
                                        .AddField(name: "Online",
                                                  notification.StreamOnline
                                                      ? "Yes"
                                                      : "No")
                                        .Build();
        DiscordMessage discordMessage = new(channel: notification.Channel, embed: embed, $"{notification.Channel} Was Followed by {notification.User}", image: null);

        await this._messageChannel.PublishAsync(message: discordMessage, cancellationToken: cancellationToken);

        this._logger.LogDebug($"{notification.Channel} Queue new follower message to Discord");
    }
}