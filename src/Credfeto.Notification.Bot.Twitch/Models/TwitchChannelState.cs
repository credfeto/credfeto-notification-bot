using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Models;

[DebuggerDisplay("{_channelName}")]
public sealed class TwitchChannelState
{
    private readonly string _channelName;
    private readonly IContributionThanks _contributionThanks;
    private readonly ILogger _logger;
    private readonly TwitchBotOptions _options;
    private readonly IRaidWelcome _raidWelcome;
    private readonly IShoutoutJoiner _shoutoutJoiner;
    private ActiveStream? _stream;

    public TwitchChannelState(string channelName,
                              TwitchBotOptions options,
                              IRaidWelcome raidWelcome,
                              IShoutoutJoiner shoutoutJoiner,
                              IContributionThanks contributionThanks,
                              [SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0024:ILogger should be typed", Justification = "Not created by DI")] ILogger logger)
    {
        this._channelName = channelName;
        this._options = options ?? throw new ArgumentNullException(nameof(options));
        this._raidWelcome = raidWelcome ?? throw new ArgumentNullException(nameof(raidWelcome));
        this._shoutoutJoiner = shoutoutJoiner ?? throw new ArgumentNullException(nameof(shoutoutJoiner));
        this._contributionThanks = contributionThanks ?? throw new ArgumentNullException(nameof(contributionThanks));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Online(string gameName, in DateTime startDate)
    {
        this._logger.LogInformation($"{this._channelName}: Going Online...");
        this._stream = new(gameName: gameName, startedAt: startDate);
    }

    public void Offline()
    {
        this._logger.LogInformation($"{this._channelName}: Going Offline...");
        this._stream = null;
    }

    public void ClearChat()
    {
        this._logger.LogInformation($"{this._channelName}: Potential incident - chat cleared.");
        this._stream?.AddIncident();
    }

    public async Task RaidedAsync(string raider, string viewerCount, CancellationToken cancellationToken)
    {
        if (this._stream?.AddRaider(raider: raider, viewerCount: viewerCount) == true && this._options.RaidWelcomeEnabled(this._channelName))
        {
            await this._raidWelcome.IssueRaidWelcomeAsync(channel: this._channelName, raider: raider, cancellationToken: cancellationToken);
        }
    }

    public async Task ChatMessageAsync(string user, string message, int bits, CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return;
        }

        if (!this._options.IsModChannel(this._channelName))
        {
            return;
        }

        if (bits != 0)
        {
            this._stream.AddBitGifter(user: user, bits: bits);

            await this._contributionThanks.ThankForBitsAsync(channel: this._channelName, user: user, cancellationToken: cancellationToken);
        }

        // TODO: Implement detection for other streamers
        if (this._stream.AddChatter(user))
        {
            // first time chatted in channel
            bool streamer = await this._shoutoutJoiner.IssueShoutoutAsync(channel: this._channelName, visitingStreamer: user, cancellationToken: cancellationToken);

            if (!streamer)
            {
                // TODO: Add new chat welcome.
            }
        }
    }

    public Task GiftedMultipleAsync(string giftedBy, int count, string months, in CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return Task.CompletedTask;
        }

        this._stream.GiftedSub(giftedBy: giftedBy, count: count);

        return this._contributionThanks.ThankForMultipleGiftSubsAsync(channelName: this._channelName, giftedBy: giftedBy, count: count, cancellationToken: cancellationToken);
    }

    public Task GiftedSubAsync(string giftedBy, string months, in CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return Task.CompletedTask;
        }

        this._stream.GiftedSub(giftedBy: giftedBy, count: 1);

        return this._contributionThanks.ThankForGiftingSubAsync(channelName: this._channelName, giftedBy: giftedBy, cancellationToken: cancellationToken);
    }

    public Task ContinuedSubAsync(string user, in CancellationToken cancellationToken)
    {
        this._stream?.ContinuedSub(user);

        return Task.CompletedTask;
    }

    public Task PrimeToPaidAsync(string user, in CancellationToken cancellationToken)
    {
        this._stream?.PrimeToPaid(user);

        return Task.CompletedTask;
    }

    public Task NewSubscriberPaidAsync(string user, in CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return Task.CompletedTask;
        }

        this._stream.NewSubscriberPaid(user);

        return this._contributionThanks.ThankForNewPaidSubAsync(channel: this._channelName, user: user, cancellationToken: cancellationToken);
    }

    public Task NewSubscriberPrimeAsync(string user, in CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return Task.CompletedTask;
        }

        this._stream.NewSubscriberPrime(user);

        return this._contributionThanks.ThankForPrimeSubAsync(channel: this._channelName, user: user, cancellationToken: cancellationToken);
    }

    public Task ResubscribePaidAsync(string user, int months, in CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return Task.CompletedTask;
        }

        this._stream.ResubscribePaid(user);

        return this._contributionThanks.ThankForPaidReSubAsync(channel: this._channelName, user: user, cancellationToken: cancellationToken);
    }

    public Task ResubscribePrimeAsync(string user, int months, in CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return Task.CompletedTask;
        }

        this._stream.ResubscribePrime(user);

        return this._contributionThanks.ThankForPrimeReSubAsync(channel: this._channelName, user: user, cancellationToken: cancellationToken);
    }
}