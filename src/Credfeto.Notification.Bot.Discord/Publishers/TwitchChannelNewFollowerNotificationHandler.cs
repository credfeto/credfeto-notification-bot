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
        string title = $"{notification.Channel} was followed by {notification.User}";

        Embed embed = new EmbedBuilder().WithColor(Color.Blue)
                                        .WithCurrentTimestamp()
                                        .WithTitle(title)
                                        .WithUrl($"https://twitch.tv/{notification.User}")
                                        .AddField(name: "Streamer",
                                                  notification.IsStreamer
                                                      ? "Yes"
                                                      : "No")
                                        .AddField(name: "Followed While Streaming",
                                                  notification.StreamOnline
                                                      ? "Yes"
                                                      : "No")
                                        .AddField(name: "Status",
                                                  notification.FollowCount == 1
                                                      ? "New Follower"
                                                      : $"Followed {notification.FollowCount} times")
                                        .AddField(name: "Account Created", notification.AccountCreated.ToString("yyyy-MM-dd HH:mm:ss"))
                                        .Build();
        DiscordMessage discordMessage = new(notification.Channel.ToString(), embed: embed, title: title, image: null);

        await this._messageChannel.PublishAsync(message: discordMessage, cancellationToken: cancellationToken);

        this._logger.LogDebug($"{notification.Channel} Queue new follower message to Discord");
    }
}