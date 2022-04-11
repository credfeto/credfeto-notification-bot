using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
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

    private readonly ConcurrentDictionary<Channel, SubDonorTracker> _donors;
    private readonly ILogger<ContributionThanks> _logger;
    private readonly TwitchBotOptions _options;

    public ContributionThanks(IOptions<TwitchBotOptions> options, IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ICurrentTimeSource currentTimeSource, ILogger<ContributionThanks> logger)
        : base(twitchChatMessageChannel)
    {
        this._currentTimeSource = currentTimeSource ?? throw new ArgumentNullException(nameof(currentTimeSource));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._donors = new();
        this._donorLock = new(1);

        this._options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task ThankForBitsAsync(Channel channel, User user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(channel);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for the bits", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for for the bits");
    }

    public async Task ThankForNewPrimeSubAsync(Channel channel, User user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(channel);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for subscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for subscribing (Prime)");
    }

    public async Task ThankForPrimeReSubAsync(Channel channel, User user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(channel);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for resubscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for resubscribing (Prime)");
    }

    public async Task ThankForPaidReSubAsync(Channel channel, User user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(channel);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for resubscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for resubscribing (Paid)");
    }

    public async Task ThankForNewPaidSubAsync(Channel channel, User user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(channel);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        await this.SendMessageAsync(channel: channel, $"Thanks @{user} for subscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: Thanks @{user} for subscribing (Paid)");
    }

    public async Task ThankForMultipleGiftSubsAsync(Channel channel, User giftedBy, int count, CancellationToken cancellationToken)
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

    public async Task ThankForGiftingSubAsync(Channel channel, User giftedBy, CancellationToken cancellationToken)
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

    public Task ThankForFollowAsync(Channel channel, User user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(channel);

        if (modChannel?.Thanks.Enabled != true)
        {
            return Task.CompletedTask;
        }

        this._logger.LogInformation($"{channel}: Thanks @{user} for following");

        return Task.CompletedTask;
    }

    private async Task<bool> WasLastSubDonorAsync(Channel channel, User giftedBy)
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

    private SubDonorTracker GetSubDonorTrackerForChannel(in Channel channel)
    {
        if (this._donors.TryGetValue(key: channel, out SubDonorTracker? subDonorTracker))
        {
            return subDonorTracker;
        }

        subDonorTracker = new(currentTimeSource: this._currentTimeSource, logger: this._logger);

        return this._donors.GetOrAdd(key: channel, value: subDonorTracker);
    }
}