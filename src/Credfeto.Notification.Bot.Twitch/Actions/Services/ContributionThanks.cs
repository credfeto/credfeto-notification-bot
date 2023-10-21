using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Date.Interfaces;
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
    private readonly ITwitchChatMessageGenerator _twitchChatMessageGenerator;

    public ContributionThanks(ITwitchChannelManager twitchChannelManager,
                              IMessageChannel<TwitchChatMessage> twitchChatMessageChannel,
                              ITwitchChatMessageGenerator twitchChatMessageGenerator,
                              ICurrentTimeSource currentTimeSource,
                              ILogger<ContributionThanks> logger)
        : base(twitchChatMessageChannel)
    {
        this._twitchChannelManager = twitchChannelManager ?? throw new ArgumentNullException(nameof(twitchChannelManager));
        this._twitchChatMessageGenerator = twitchChatMessageGenerator ?? throw new ArgumentNullException(nameof(twitchChatMessageGenerator));
        this._currentTimeSource = currentTimeSource ?? throw new ArgumentNullException(nameof(currentTimeSource));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._donors = new();
        this._donorLock = new(1);
    }

    public async Task ThankForBitsAsync(Streamer streamer, Viewer user, int bitsGiven, CancellationToken cancellationToken)
    {
        if (!this.IsThanksEnabled(streamer))
        {
            return;
        }

        string message = this._twitchChatMessageGenerator.ThanksForBits(giftedBy: user, bitsGiven: bitsGiven);
        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, message: message, cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{user} for for the bits");
    }

    public async Task ThankForNewPrimeSubAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        if (!this.IsThanksEnabled(streamer))
        {
            return;
        }

        string message = this._twitchChatMessageGenerator.ThanksForNewPrimeSub(user);
        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, message: message, cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{user} for subscribing (Prime)");
    }

    public async Task ThankForPrimeReSubAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        if (!this.IsThanksEnabled(streamer))
        {
            return;
        }

        string message = this._twitchChatMessageGenerator.ThanksForPrimeReSub(user);
        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, message: message, cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{user} for resubscribing (Prime)");
    }

    public async Task ThankForPaidReSubAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        if (!this.IsThanksEnabled(streamer))
        {
            return;
        }

        string message = this._twitchChatMessageGenerator.ThanksForPaidReSub(user);
        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, message: message, cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{user} for resubscribing (Paid)");
    }

    public async Task ThankForNewPaidSubAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        if (!this.IsThanksEnabled(streamer))
        {
            return;
        }

        string message = this._twitchChatMessageGenerator.ThanksForNewPaidSub(user);
        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, message: message, cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{user} for subscribing (Paid)");
    }

    public async Task ThankForMultipleGiftSubsAsync(Streamer streamer, Viewer giftedBy, int count, CancellationToken cancellationToken)
    {
        if (!this.IsThanksEnabled(streamer))
        {
            return;
        }

        if (await this.WasLastSubDonorAsync(streamer: streamer, giftedBy: giftedBy, cancellationToken: cancellationToken))
        {
            this._logger.LogInformation($"{streamer}: Thanks @{giftedBy} for gifting sub (Same as last donor).");

            return;
        }

        string message = this._twitchChatMessageGenerator.ThanksForGiftingMultipleSubs(giftedBy);
        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, message: message, cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{giftedBy} for gifting subs");
    }

    public async Task ThankForGiftingSubAsync(Streamer streamer, Viewer giftedBy, CancellationToken cancellationToken)
    {
        if (!this.IsThanksEnabled(streamer))
        {
            return;
        }

        this._logger.LogInformation($"{streamer}: Thanks @{giftedBy} for gifting sub");

        if (await this.WasLastSubDonorAsync(streamer: streamer, giftedBy: giftedBy, cancellationToken: cancellationToken))
        {
            this._logger.LogInformation($"{streamer}: Thanks @{giftedBy} for gifting sub (Same as last donor)");

            return;
        }

        string message = this._twitchChatMessageGenerator.ThanksForGiftingOneSub(giftedBy);
        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, message: message, cancellationToken: cancellationToken);
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

    private async Task<bool> WasLastSubDonorAsync(Streamer streamer, Viewer giftedBy, CancellationToken cancellationToken)
    {
        await this._donorLock.WaitAsync(cancellationToken);

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