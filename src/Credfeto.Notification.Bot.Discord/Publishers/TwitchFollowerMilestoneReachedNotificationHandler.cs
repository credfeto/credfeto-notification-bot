using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Discord.Models;
using Credfeto.Notification.Bot.Discord.Publishers.Logging;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Models;
using Discord;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Discord.Publishers;

public sealed class TwitchFollowerMilestoneReachedNotificationHandler : INotificationHandler<TwitchFollowerMilestoneReached>
{
    private readonly ILogger<TwitchFollowerMilestoneReachedNotificationHandler> _logger;
    private readonly IMessageChannel<DiscordMessage> _messageChannel;

    public TwitchFollowerMilestoneReachedNotificationHandler(IMessageChannel<DiscordMessage> messageChannel, ILogger<TwitchFollowerMilestoneReachedNotificationHandler> logger)
    {
        this._messageChannel = messageChannel ?? throw new ArgumentNullException(nameof(messageChannel));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask Handle(TwitchFollowerMilestoneReached notification, CancellationToken cancellationToken)
    {
        string title = $"Woo! New follower milestone reached {notification.MilestoneReached}";

        Embed embed = BuildEmbed(notification: notification, title: title);

        DiscordMessage discordMessage = new(notification.Streamer.ToString(), embed: embed, title: title, image: null);

        await this._messageChannel.PublishAsync(message: discordMessage, cancellationToken: cancellationToken);

        this._logger.LogFolloweMilestoneReached(streamer: notification.Streamer, milestoneReached: notification.MilestoneReached);
    }

    private static Embed BuildEmbed(TwitchFollowerMilestoneReached notification, string title)
    {
        return new EmbedBuilder().WithColor(Color.Gold)
                                 .WithTitle(title)
                                 .WithCurrentTimestamp()
                                 .WithUrl($"https://twitch.tv/{notification.Streamer}")
                                 .AddField(name: "Next Milestone", value: notification.NextMilestone)
                                 .Build();
    }
}