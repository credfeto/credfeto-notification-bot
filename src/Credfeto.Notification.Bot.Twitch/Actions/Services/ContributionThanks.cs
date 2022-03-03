using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Models;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class ContributionThanks : MessageSenderBase, IContributionThanks
{
    private readonly ILogger<ContributionThanks> _logger;

    public ContributionThanks(IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ILogger<ContributionThanks> logger)
        : base(twitchChatMessageChannel)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Welcome to everyone from front page; If you're enjoying what you're seeing and hearing then click through and follow.
    public async Task ThankForBitsAsync(string channel, string user, CancellationToken cancellationToken)
    {
        await this.SendMessageAsync(channel: channel, $"Thanks @{user} bis.", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for bits.");
    }

    public async Task ThankForPrimeSubAsync(string channel, string user, CancellationToken cancellationToken)
    {
        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for subscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for subscribing (Prime)");
    }

    public async Task ThankForPrimeReSubAsync(string channel, string user, CancellationToken cancellationToken)
    {
        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for resubscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for resubscribing (Prime)");
    }

    public async Task ThankForPaidReSubAsync(string channel, string user, CancellationToken cancellationToken)
    {
        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for resubscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for resubscribing (Paid)");
    }

    public async Task ThankForNewPaidSubAsync(string channel, string user, CancellationToken cancellationToken)
    {
        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for subscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for subscribing (Paid)");
    }

    public Task ThankForMultipleGiftSubsAsync(string channelName, string giftedBy, int count, CancellationToken cancellationToken)
    {
        //await this.SendMessageAsync(channel: channelName, $"Thanks @{giftedBy} for gifting subs.", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channelName}: Thanks @{giftedBy} for gifting subs.");

        return Task.CompletedTask;
    }

    public Task ThankForGiftingSubAsync(string channelName, string giftedBy, CancellationToken cancellationToken)
    {
        // await this.SendMessageAsync(channel: channelName, $"Thanks @{giftedBy} for gifting sub.", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channelName}: Thanks @{giftedBy} for gifting sub.");

        return Task.CompletedTask;
    }
}