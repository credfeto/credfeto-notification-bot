using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.StreamState;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NonBlocking;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class ContributionThanks : MessageSenderBase, IContributionThanks
{
    private readonly ICurrentTimeSource _currentTimeSource;
    private readonly SemaphoreSlim _donorLock;

    private readonly ConcurrentDictionary<string, SubDonorTracker> _donors;
    private readonly ILogger<ContributionThanks> _logger;
    private readonly TwitchBotOptions _options;

    public ContributionThanks(IOptions<TwitchBotOptions> options, IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ICurrentTimeSource currentTimeSource, ILogger<ContributionThanks> logger)
        : base(twitchChatMessageChannel)
    {
        this._currentTimeSource = currentTimeSource ?? throw new ArgumentNullException(nameof(currentTimeSource));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._donors = new(StringComparer.CurrentCultureIgnoreCase);
        this._donorLock = new(1);

        this._options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task ThankForBitsAsync(string channel, string user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(channel);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for the bits", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for for the bits");
    }

    public async Task ThankForNewPrimeSubAsync(string channel, string user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(channel);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for subscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for subscribing (Prime)");
    }

    public async Task ThankForPrimeReSubAsync(string channel, string user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(channel);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for resubscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for resubscribing (Prime)");
    }

    public async Task ThankForPaidReSubAsync(string channel, string user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(channel);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for resubscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for resubscribing (Paid)");
    }

    public async Task ThankForNewPaidSubAsync(string channel, string user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(channel);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for subscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for subscribing (Paid)");
    }

    public async Task ThankForMultipleGiftSubsAsync(string channel, string giftedBy, int count, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(channel);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        if (await this.WasLastSubDonorAsync(channel: channel, giftedBy: giftedBy))
        {
            this._logger.LogInformation($"{channel}: Thanks @{giftedBy} for gifting sub (Same as last donor).");

            return;
        }

        await this.SendMessageAsync(channel: channel, $"Thanks @{giftedBy} for gifting subs", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{giftedBy} for gifting subs");
    }

    public async Task ThankForGiftingSubAsync(string channel, string giftedBy, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(channel);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        this._logger.LogInformation($"{channel}: Thanks @{giftedBy} for gifting sub");

        if (await this.WasLastSubDonorAsync(channel: channel, giftedBy: giftedBy))
        {
            this._logger.LogInformation($"{channel}: Thanks @{giftedBy} for gifting sub (Same as last donor)");

            return;
        }

        await this.SendMessageAsync(channel: channel, $"Thanks @{giftedBy} for gifting sub", cancellationToken: cancellationToken);
    }

    public Task ThankForFollowAsync(string channel, string user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(channel);

        if (modChannel?.Thanks.Enabled != true)
        {
            return Task.CompletedTask;
        }

        this._logger.LogInformation($"{channel}: Thanks @{user} for following");

        return Task.CompletedTask;
    }

    private async Task<bool> WasLastSubDonorAsync(string channel, string giftedBy)
    {
        await this._donorLock.WaitAsync();

        try
        {
            SubDonorTracker subDonorTracker = this.GetSubDonorTrackerForChannel(channel);

            return subDonorTracker.Update(giftedBy);
        }
        finally
        {
            this._donorLock.Release();
        }
    }

    private SubDonorTracker GetSubDonorTrackerForChannel(string channel)
    {
        if (this._donors.TryGetValue(key: channel, out SubDonorTracker? subDonorTracker))
        {
            return subDonorTracker;
        }

        subDonorTracker = new(currentTimeSource: this._currentTimeSource, logger: this._logger);

        return this._donors.GetOrAdd(key: channel, value: subDonorTracker);
    }
}