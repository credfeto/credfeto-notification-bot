using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.StreamState;
using Microsoft.Extensions.Logging;
using NonBlocking;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class ContributionThanks : MessageSenderBase, IContributionThanks
{
    private readonly ICurrentTimeSource _currentTimeSource;
    private readonly SemaphoreSlim _donorLock;

    private readonly ConcurrentDictionary<Streamer, SubDonorTracker> _donors;
    private readonly ILogger<ContributionThanks> _logger;
    private readonly ITwitchChannelManager _twitchChannelManager;

    public ContributionThanks(ITwitchChannelManager twitchChannelManager,
                              IMessageChannel<TwitchChatMessage> twitchChatMessageChannel,
                              ICurrentTimeSource currentTimeSource,
                              ILogger<ContributionThanks> logger)
        : base(twitchChatMessageChannel)
    {
        this._twitchChannelManager = twitchChannelManager ?? throw new ArgumentNullException(nameof(twitchChannelManager));
        this._currentTimeSource = currentTimeSource ?? throw new ArgumentNullException(nameof(currentTimeSource));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._donors = new();
        this._donorLock = new(1);
    }

    public async Task ThankForBitsAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        if (!this.IsThanksEnabled(streamer))
        {
            return;
        }

        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, $"Thanks @{user} for the bits", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{user} for for the bits");
    }

    public async Task ThankForNewPrimeSubAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        if (!this.IsThanksEnabled(streamer))
        {
            return;
        }

        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, $"Thanks @{user} for subscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{user} for subscribing (Prime)");
    }

    public async Task ThankForPrimeReSubAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        if (!this.IsThanksEnabled(streamer))
        {
            return;
        }

        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, $"Thanks @{user} for resubscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{user} for resubscribing (Prime)");
    }

    public async Task ThankForPaidReSubAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        if (!this.IsThanksEnabled(streamer))
        {
            return;
        }

        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, $"Thanks @{user} for resubscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{user} for resubscribing (Paid)");
    }

    public async Task ThankForNewPaidSubAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        if (!this.IsThanksEnabled(streamer))
        {
            return;
        }

        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, $"Thanks @{user} for subscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{user} for subscribing (Paid)");
    }

    public async Task ThankForMultipleGiftSubsAsync(Streamer streamer, Viewer giftedBy, int count, CancellationToken cancellationToken)
    {
        if (!this.IsThanksEnabled(streamer))
        {
            return;
        }

        if (await this.WasLastSubDonorAsync(streamer: streamer, giftedBy: giftedBy))
        {
            this._logger.LogInformation($"{streamer}: Thanks @{giftedBy} for gifting sub (Same as last donor).");

            return;
        }

        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, $"Thanks @{giftedBy} for gifting subs", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{giftedBy} for gifting subs");
    }

    public async Task ThankForGiftingSubAsync(Streamer streamer, Viewer giftedBy, CancellationToken cancellationToken)
    {
        if (!this.IsThanksEnabled(streamer))
        {
            return;
        }

        this._logger.LogInformation($"{streamer}: Thanks @{giftedBy} for gifting sub");

        if (await this.WasLastSubDonorAsync(streamer: streamer, giftedBy: giftedBy))
        {
            this._logger.LogInformation($"{streamer}: Thanks @{giftedBy} for gifting sub (Same as last donor)");

            return;
        }

        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, $"Thanks @{giftedBy} for gifting sub", cancellationToken: cancellationToken);
    }

    public Task ThankForFollowAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        if (!this.IsThanksEnabled(streamer))
        {
            return Task.CompletedTask;
        }

        this._logger.LogInformation($"{streamer}: Thanks @{user} for following");

        return Task.CompletedTask;
    }

    private bool IsThanksEnabled(in Streamer streamer)
    {
        return this._twitchChannelManager.GetStreamer(streamer)
                   .Settings.ThanksEnabled;
    }

    private async Task<bool> WasLastSubDonorAsync(Streamer streamer, Viewer giftedBy)
    {
        await this._donorLock.WaitAsync();

        try
        {
            SubDonorTracker subDonorTracker = this.GetSubDonorTrackerForChannel(streamer);

            return subDonorTracker.Update(giftedBy);
        }
        finally
        {
            this._donorLock.Release();
        }
    }

    private SubDonorTracker GetSubDonorTrackerForChannel(in Streamer streamer)
    {
        if (this._donors.TryGetValue(key: streamer, out SubDonorTracker? subDonorTracker))
        {
            return subDonorTracker;
        }

        subDonorTracker = new(currentTimeSource: this._currentTimeSource, logger: this._logger);

        return this._donors.GetOrAdd(key: streamer, value: subDonorTracker);
    }
}