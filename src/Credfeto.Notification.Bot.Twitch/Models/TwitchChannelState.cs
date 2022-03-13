using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Models;

[DebuggerDisplay("{_channelName}")]
public sealed class TwitchChannelState
{
    private readonly IChannelFollowCount _channelFollowCount;
    private readonly string _channelName;
    private readonly IContributionThanks _contributionThanks;
    private readonly IFollowerMilestone _followerMilestone;
    private readonly ILogger _logger;
    private readonly TwitchBotOptions _options;
    private readonly IRaidWelcome _raidWelcome;
    private readonly IShoutoutJoiner _shoutoutJoiner;
    private readonly ITwitchStreamDataManager _twitchStreamDataManager;
    private readonly IUserInfoService _userInfoService;

    private ActiveStream? _stream;

    public TwitchChannelState(string channelName,
                              TwitchBotOptions options,
                              IRaidWelcome raidWelcome,
                              IShoutoutJoiner shoutoutJoiner,
                              IContributionThanks contributionThanks,
                              IUserInfoService userInfoService,
                              ITwitchStreamDataManager twitchStreamDataManager,
                              IChannelFollowCount channelFollowCount,
                              IFollowerMilestone followerMilestone,
                              [SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0024:ILogger should be typed", Justification = "Not created by DI")] ILogger logger)
    {
        this._channelName = channelName;
        this._options = options ?? throw new ArgumentNullException(nameof(options));
        this._raidWelcome = raidWelcome ?? throw new ArgumentNullException(nameof(raidWelcome));
        this._shoutoutJoiner = shoutoutJoiner ?? throw new ArgumentNullException(nameof(shoutoutJoiner));
        this._contributionThanks = contributionThanks ?? throw new ArgumentNullException(nameof(contributionThanks));
        this._userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        this._twitchStreamDataManager = twitchStreamDataManager ?? throw new ArgumentNullException(nameof(twitchStreamDataManager));
        this._channelFollowCount = channelFollowCount ?? throw new ArgumentNullException(nameof(channelFollowCount));
        this._followerMilestone = followerMilestone ?? throw new ArgumentNullException(nameof(followerMilestone));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task OnlineAsync(string gameName, in DateTime startDate)
    {
        this._logger.LogInformation($"{this._channelName}: Going Online...");
        this._stream = new(gameName: gameName, startedAt: startDate);

        return this._twitchStreamDataManager.RecordStreamStartAsync(channel: this._channelName, streamStartDate: startDate);
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
            this._logger.LogDebug($"{this._channelName}: Message from {user} while stream offline");

            return;
        }

        if (!this._options.IsModChannel(this._channelName))
        {
            this._logger.LogDebug($"{this._channelName}: Message from {user} that not modding for");

            return;
        }

        if (bits != 0)
        {
            this._logger.LogDebug($"{this._channelName}: {user} Gave {bits}");
            this._stream.AddBitGifter(user: user, bits: bits);

            if (!this._options.IsSelf(user))
            {
                await this._contributionThanks.ThankForBitsAsync(channel: this._channelName, user: user, cancellationToken: cancellationToken);
            }
        }

        // TODO: Implement detection for other streamers
        if (this._stream.AddChatter(user))
        {
            // note that this covers disconnections of the bot
            bool firstTimeInStream = await this._twitchStreamDataManager.IsFirstMessageInStreamAsync(channel: this._channelName, streamStartDate: this._stream.StartedAt, username: user);

            if (!firstTimeInStream)
            {
                return;
            }

            await this._twitchStreamDataManager.AddChatterToStreamAsync(channel: this._channelName, streamStartDate: this._stream.StartedAt, username: user);

            TwitchUser? twitchUser = await this._userInfoService.GetUserAsync(user);

            if (twitchUser == null)
            {
                return;
            }

            this._logger.LogWarning($"Found {twitchUser.UserName}. Streamer: {twitchUser.IsStreamer} Created: {twitchUser.DateCreated}");

            // TODO: Add new chat welcome (To regulars?).

            if (twitchUser.IsStreamer)
            {
                await this._shoutoutJoiner.IssueShoutoutAsync(channel: this._channelName, visitingStreamer: twitchUser, cancellationToken: cancellationToken);
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

        if (this._options.IsSelf(giftedBy))
        {
            return Task.CompletedTask;
        }

        return this._contributionThanks.ThankForGiftingSubAsync(channelName: this._channelName, giftedBy: giftedBy, cancellationToken: cancellationToken);
    }

    public Task ContinuedSubAsync(string user, in CancellationToken cancellationToken)
    {
        this._stream?.ContinuedSub(user);

        if (this._options.IsSelf(user))
        {
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    public Task PrimeToPaidAsync(string user, in CancellationToken cancellationToken)
    {
        this._stream?.PrimeToPaid(user);

        if (this._options.IsSelf(user))
        {
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    public Task NewSubscriberPaidAsync(string user, in CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return Task.CompletedTask;
        }

        this._stream.NewSubscriberPaid(user);

        if (this._options.IsSelf(user))
        {
            return Task.CompletedTask;
        }

        return this._contributionThanks.ThankForNewPaidSubAsync(channel: this._channelName, user: user, cancellationToken: cancellationToken);
    }

    public Task NewSubscriberPrimeAsync(string user, in CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return Task.CompletedTask;
        }

        this._stream.NewSubscriberPrime(user);

        if (this._options.IsSelf(user))
        {
            return Task.CompletedTask;
        }

        return this._contributionThanks.ThankForPrimeSubAsync(channel: this._channelName, user: user, cancellationToken: cancellationToken);
    }

    public Task ResubscribePaidAsync(string user, int months, in CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return Task.CompletedTask;
        }

        this._stream.ResubscribePaid(user);

        if (this._options.IsSelf(user))
        {
            return Task.CompletedTask;
        }

        return this._contributionThanks.ThankForPaidReSubAsync(channel: this._channelName, user: user, cancellationToken: cancellationToken);
    }

    public Task ResubscribePrimeAsync(string user, int months, in CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return Task.CompletedTask;
        }

        this._stream.ResubscribePrime(user);

        if (this._options.IsSelf(user))
        {
            return Task.CompletedTask;
        }

        return this._contributionThanks.ThankForPrimeReSubAsync(channel: this._channelName, user: user, cancellationToken: cancellationToken);
    }

    public async Task NewFollowerAsync(string user, CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return;
        }

        if (this._stream.Follow(user))
        {
            await this._contributionThanks.ThankForFollowAsync(channelName: this._channelName, user: user, cancellationToken: cancellationToken);

            int followers = await this._channelFollowCount.GetCurrentFollowerCountAsync(username: this._channelName, cancellationToken: cancellationToken);

            await this._followerMilestone.IssueMilestoneUpdateAsync(channel: this._channelName, followers: followers, cancellationToken: cancellationToken);
        }
    }
}