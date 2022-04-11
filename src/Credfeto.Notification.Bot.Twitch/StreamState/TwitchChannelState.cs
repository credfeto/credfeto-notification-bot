using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.StreamState;

[DebuggerDisplay("{_channelName}")]
public sealed class TwitchChannelState : ITwitchChannelState
{
    private readonly Channel _channelName;
    private readonly ILogger _logger;
    private readonly IMediator _mediator;
    private readonly TwitchBotOptions _options;
    private readonly ITwitchStreamDataManager _twitchStreamDataManager;
    private readonly IUserInfoService _userInfoService;

    private ActiveStream? _stream;

    public TwitchChannelState(in Channel channelName,
                              TwitchBotOptions options,
                              IUserInfoService userInfoService,
                              ITwitchStreamDataManager twitchStreamDataManager,
                              IMediator mediator,
                              [SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0024:ILogger should be typed", Justification = "Not created by DI")] ILogger logger)
    {
        this._channelName = channelName;
        this._options = options ?? throw new ArgumentNullException(nameof(options));
        this._userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        this._twitchStreamDataManager = twitchStreamDataManager ?? throw new ArgumentNullException(nameof(twitchStreamDataManager));
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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

    public async Task RaidedAsync(User raider, int viewerCount, CancellationToken cancellationToken)
    {
        if (this._stream?.AddRaider(raider: raider, viewerCount: viewerCount) == true && this._options.RaidWelcomeEnabled(this._channelName))
        {
            await this._mediator.Publish(new TwitchStreamRaided(channel: this._channelName, raider: raider, viewerCount: viewerCount), cancellationToken: cancellationToken);
        }
    }

    public async Task ChatMessageAsync(User user, string message, int bits, CancellationToken cancellationToken)
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

        if (this._options.IsSelf(user))
        {
            return;
        }

        if (StringComparer.InvariantCultureIgnoreCase.Equals(x: this._channelName, y: user))
        {
            this._logger.LogDebug($"{this._channelName}: Message from {user} that not modding for");

            return;
        }

        if (bits != 0)
        {
            this._logger.LogDebug($"{this._channelName}: {user} Gave {bits}");
            this._stream.AddBitGifter(user: user, bits: bits);

            await this._mediator.Publish(new TwitchBitsGift(channel: this._channelName, user: user, bits: bits), cancellationToken: cancellationToken);
        }

        if (this._options.IsIgnoredUser(user))
        {
            return;
        }

        if (!this._stream.AddChatter(user))
        {
            return;
        }

        // note that this covers disconnections of the bot
        bool firstTimeInStream = await this._twitchStreamDataManager.IsFirstMessageInStreamAsync(channel: this._channelName, streamStartDate: this._stream.StartedAt, username: user);

        if (!firstTimeInStream)
        {
            return;
        }

        bool isRegular = await this.IsRegularChatterAsync(channel: this._channelName, username: user);

        await this._twitchStreamDataManager.AddChatterToStreamAsync(channel: this._channelName, streamStartDate: this._stream.StartedAt, username: user);

        await this._mediator.Publish(new TwitchStreamNewChatter(channel: this._channelName, user: user, isRegular: isRegular), cancellationToken: cancellationToken);
    }

    public Task GiftedMultipleAsync(User giftedBy, int count, string months, in CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return Task.CompletedTask;
        }

        this._stream.GiftedSub(giftedBy: giftedBy, count: count);

        return this._mediator.Publish(new TwitchGiftSubMultiple(channel: this._channelName, user: giftedBy, count: count), cancellationToken: cancellationToken);
    }

    public Task GiftedSubAsync(User giftedBy, string months, in CancellationToken cancellationToken)
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

        return this._mediator.Publish(new TwitchGiftSubSingle(channel: this._channelName, user: giftedBy), cancellationToken: cancellationToken);
    }

    public Task ContinuedSubAsync(User user, in CancellationToken cancellationToken)
    {
        this._stream?.ContinuedSub(user);

        if (this._options.IsSelf(user))
        {
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    public Task PrimeToPaidAsync(User user, in CancellationToken cancellationToken)
    {
        this._stream?.PrimeToPaid(user);

        if (this._options.IsSelf(user))
        {
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    public Task NewSubscriberPaidAsync(User user, in CancellationToken cancellationToken)
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

        return this._mediator.Publish(new TwitchNewPaidSub(channel: this._channelName, user: user), cancellationToken: cancellationToken);
    }

    public Task NewSubscriberPrimeAsync(User user, in CancellationToken cancellationToken)
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

        return this._mediator.Publish(new TwitchNewPrimeSub(channel: this._channelName, user: user), cancellationToken: cancellationToken);
    }

    public Task ResubscribePaidAsync(User user, int months, in CancellationToken cancellationToken)
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

        return this._mediator.Publish(new TwitchPaidReSub(channel: this._channelName, user: user), cancellationToken: cancellationToken);
    }

    public Task ResubscribePrimeAsync(User user, int months, in CancellationToken cancellationToken)
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

        return this._mediator.Publish(new TwitchPrimeReSub(channel: this._channelName, user: user), cancellationToken: cancellationToken);
    }

    public async Task NewFollowerAsync(User user, CancellationToken cancellationToken)
    {
        int followCount = await this._twitchStreamDataManager.RecordNewFollowerAsync(channel: this._channelName, username: user);

        TwitchUser? twitchUser = await this._userInfoService.GetUserAsync(user);

        TwitchChannelNewFollower model;

        if (twitchUser != null)
        {
            model = new(channel: this._channelName, user: user, this._stream != null, isStreamer: twitchUser.IsStreamer, accountCreated: twitchUser.DateCreated, followCount: followCount);
        }
        else
        {
            model = new(channel: this._channelName, user: user, this._stream != null, isStreamer: false, accountCreated: DateTime.MinValue, followCount: followCount);
        }

        await this._mediator.Publish(notification: model, cancellationToken: cancellationToken);
    }

    private async Task<bool> IsRegularChatterAsync(Channel channel, User username)
    {
        try
        {
            return await this._twitchStreamDataManager.IsRegularChatterAsync(channel: channel, username: username);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{channel}: Is Regular Chatter: Failed to check {exception.Message}");

            return false;
        }
    }
}