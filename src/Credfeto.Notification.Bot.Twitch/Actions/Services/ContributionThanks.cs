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

    private readonly ConcurrentDictionary<Streamer, SubDonorTracker> _donors;
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

    public async Task ThankForBitsAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(streamer);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        await this.SendMessageAsync(streamer: streamer, $"Thanks @{user} for the bits", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{user} for for the bits");
    }

    public async Task ThankForNewPrimeSubAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(streamer);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        await this.SendMessageAsync(streamer: streamer, $"Thanks @{user} for subscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{user} for subscribing (Prime)");
    }

    public async Task ThankForPrimeReSubAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(streamer);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        await this.SendMessageAsync(streamer: streamer, $"Thanks @{user} for resubscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{user} for resubscribing (Prime)");
    }

    public async Task ThankForPaidReSubAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(streamer);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        await this.SendMessageAsync(streamer: streamer, $"Thanks @{user} for resubscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{user} for resubscribing (Paid)");
    }

    public async Task ThankForNewPaidSubAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(streamer);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        await this.SendMessageAsync(streamer: streamer, $"Thanks @{user} for subscribing", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{user} for subscribing (Paid)");
    }

    public async Task ThankForMultipleGiftSubsAsync(Streamer streamer, Viewer giftedBy, int count, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(streamer);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        if (await this.WasLastSubDonorAsync(streamer: streamer, giftedBy: giftedBy))
        {
            this._logger.LogInformation($"{streamer}: Thanks @{giftedBy} for gifting sub (Same as last donor).");

            return;
        }

        await this.SendMessageAsync(streamer: streamer, $"Thanks @{giftedBy} for gifting subs", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: Thanks @{giftedBy} for gifting subs");
    }

    public async Task ThankForGiftingSubAsync(Streamer streamer, Viewer giftedBy, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(streamer);

        if (modChannel?.Thanks.Enabled != true)
        {
            return;
        }

        this._logger.LogInformation($"{streamer}: Thanks @{giftedBy} for gifting sub");

        if (await this.WasLastSubDonorAsync(streamer: streamer, giftedBy: giftedBy))
        {
            this._logger.LogInformation($"{streamer}: Thanks @{giftedBy} for gifting sub (Same as last donor)");

            return;
        }

        await this.SendMessageAsync(streamer: streamer, $"Thanks @{giftedBy} for gifting sub", cancellationToken: cancellationToken);
    }

    public Task ThankForFollowAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(streamer);

        if (modChannel?.Thanks.Enabled != true)
        {
            return Task.CompletedTask;
        }

        this._logger.LogInformation($"{streamer}: Thanks @{user} for following");

        return Task.CompletedTask;
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