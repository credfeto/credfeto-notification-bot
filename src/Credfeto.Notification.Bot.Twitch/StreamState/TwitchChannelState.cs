using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Services;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.StreamState;

[DebuggerDisplay("{Streamer}")]
public sealed class TwitchChannelState : ITwitchChannelState
{
    private readonly ILogger _logger;
    private readonly IMediator _mediator;
    private readonly ITwitchStreamSettings _offlineStreamSettings;
    private readonly TwitchBotOptions _options;
    private readonly ITwitchStreamDataManager _twitchStreamDataManager;
    private readonly IUserInfoService _userInfoService;

    private ActiveStream? _stream;

    public TwitchChannelState(in Streamer streamerStreamer,
                              TwitchBotOptions options,
                              IUserInfoService userInfoService,
                              ITwitchStreamDataManager twitchStreamDataManager,
                              IMediator mediator,
                              [SuppressMessage(category: "FunFair.CodeAnalysis", checkId: "FFS0024:ILogger should be typed", Justification = "Not created by DI")] ILogger logger)
    {
        this.Streamer = streamerStreamer;
        this._options = options ?? throw new ArgumentNullException(nameof(options));
        this._userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        this._twitchStreamDataManager = twitchStreamDataManager ?? throw new ArgumentNullException(nameof(twitchStreamDataManager));
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._offlineStreamSettings = new TwitchStreamSettingsOffline(options: this._options, streamer: this.Streamer);
    }

    public Streamer Streamer { get; }

    public bool IsOnline => this._stream != null;

    public async ValueTask OnlineAsync(string gameName, DateTimeOffset startDate, CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{this.Streamer}: Going Online...");
        ActiveStream stream = new(gameName: gameName, startedAt: startDate, new TwitchStreamSettingsOnline(options: this._options, streamer: this.Streamer, logger: this._logger));
        this._stream = stream;
        await this._twitchStreamDataManager.RecordStreamStartAsync(streamer: this.Streamer, streamStartDate: startDate, cancellationToken: cancellationToken);

        StreamSettings? settings = await this._twitchStreamDataManager.GetSettingsAsync(streamer: this.Streamer, streamStartDate: startDate, cancellationToken: cancellationToken);

        if (settings != null)
        {
            stream.Settings.OverrideWelcomes(settings.ChatWelcomesEnabled);
            stream.Settings.OverrideRaidWelcomes(settings.RaidWelcomesEnabled);
            stream.Settings.OverrideMilestonesEnabled(settings.AnnounceMilestonesEnabled);
            stream.Settings.OverrideShoutOuts(settings.ShoutOutsEnabled);
            stream.Settings.OverrideThanks(settings.ThanksEnabled);

            if (settings.ChatWelcomesEnabled != stream.Settings.ChatWelcomesEnabled || settings.RaidWelcomesEnabled != stream.Settings.RaidWelcomesEnabled ||
                settings.AnnounceMilestonesEnabled != stream.Settings.AnnounceMilestonesEnabled || settings.ShoutOutsEnabled != stream.Settings.ShoutOutsEnabled ||
                settings.ThanksEnabled != stream.Settings.ThanksEnabled)
            {
                await this.SaveStreamSettingsInternalAsync(stream: stream, cancellationToken: cancellationToken);
            }
        }
    }

    public ValueTask SaveStreamSettingsAsync(in CancellationToken cancellationToken)
    {
        ActiveStream? stream = this._stream;

        return stream == null
            ? ValueTask.CompletedTask
            : this.SaveStreamSettingsInternalAsync(stream: stream, cancellationToken: cancellationToken);
    }

    public void Offline()
    {
        this._logger.LogInformation($"{this.Streamer}: Going Offline...");
        this._stream = null;
    }

    public void ClearChat()
    {
        this._logger.LogInformation($"{this.Streamer}: Potential incident - chat cleared.");
        this._stream?.AddIncident();
    }

    public async ValueTask RaidedAsync(Viewer raider, int viewerCount, CancellationToken cancellationToken)
    {
        if (this._stream?.AddRaider(raider: raider, viewerCount: viewerCount) == true)
        {
            await this._mediator.Publish(new TwitchStreamRaided(streamer: this.Streamer, raider: raider, viewerCount: viewerCount), cancellationToken: cancellationToken);
        }
    }

    public async ValueTask ChatMessageAsync(Viewer user, string message, CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            this._logger.LogDebug($"{this.Streamer}: Message from {user} while stream offline");

            return;
        }

        if (!this._options.IsModChannel(this.Streamer))
        {
            this._logger.LogDebug($"{this.Streamer}: Message from {user} that not modding for");

            return;
        }

        if (this._options.IsSelf(user))
        {
            return;
        }

        if (this.Streamer == user.ToStreamer())
        {
            this._logger.LogDebug($"{this.Streamer}: Message from streamer themselves");

            return;
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
        bool firstTimeInStream =
            await this._twitchStreamDataManager.IsFirstMessageInStreamAsync(streamer: this.Streamer,
                                                                            streamStartDate: this._stream.StartedAt,
                                                                            username: user,
                                                                            cancellationToken: cancellationToken);

        if (!firstTimeInStream)
        {
            return;
        }

        bool isRegular = await this.IsRegularChatterAsync(streamer: this.Streamer, username: user, cancellationToken: cancellationToken);

        TwitchUser? twitchUser = await this._userInfoService.GetUserAsync(userName: user, cancellationToken: cancellationToken);

        if (twitchUser != null)
        {
            await this._twitchStreamDataManager.AddChatterToStreamAsync(streamer: this.Streamer,
                                                                        streamStartDate: this._stream.StartedAt,
                                                                        username: user,
                                                                        cancellationToken: cancellationToken);
        }

        // no point in welcoming ignored users
        await this._mediator.Publish(new TwitchStreamNewChatter(streamer: this.Streamer, user: user, isRegular: isRegular), cancellationToken: cancellationToken);
    }

    public async ValueTask BitsGiftedAsync(Viewer user, int bits, CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            this._logger.LogDebug($"{this.Streamer}: Message from {user} while stream offline");

            return;
        }

        if (!this._options.IsModChannel(this.Streamer))
        {
            this._logger.LogDebug($"{this.Streamer}: Message from {user} that not modding for");

            return;
        }

        if (this._options.IsSelf(user))
        {
            return;
        }

        this._logger.LogDebug($"{this.Streamer}: {user} Gave {bits}");
        this._stream.AddBitGifter(user: user, bits: bits);

        await this._mediator.Publish(new TwitchBitsGift(streamer: this.Streamer, user: user, bits: bits), cancellationToken: cancellationToken);
    }

    public ValueTask GiftedMultipleAsync(Viewer giftedBy, int count, string months, in CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return ValueTask.CompletedTask;
        }

        this._stream.GiftedSub(giftedBy: giftedBy, count: count);

        return this._mediator.Publish(new TwitchGiftSubMultiple(streamer: this.Streamer, user: giftedBy, count: count), cancellationToken: cancellationToken);
    }

    public ValueTask GiftedSubAsync(Viewer giftedBy, string months, in CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return ValueTask.CompletedTask;
        }

        this._stream.GiftedSub(giftedBy: giftedBy, count: 1);

        if (this._options.IsSelf(giftedBy))
        {
            return ValueTask.CompletedTask;
        }

        return this._mediator.Publish(new TwitchGiftSubSingle(streamer: this.Streamer, user: giftedBy), cancellationToken: cancellationToken);
    }

    public ValueTask ContinuedSubAsync(Viewer user, in CancellationToken cancellationToken)
    {
        this._stream?.ContinuedSub(user);

        if (this._options.IsSelf(user))
        {
            return ValueTask.CompletedTask;
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask PrimeToPaidAsync(Viewer user, in CancellationToken cancellationToken)
    {
        this._stream?.PrimeToPaid(user);

        if (this._options.IsSelf(user))
        {
            return ValueTask.CompletedTask;
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask NewSubscriberPaidAsync(Viewer user, in CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return ValueTask.CompletedTask;
        }

        this._stream.NewSubscriberPaid(user);

        if (this._options.IsSelf(user))
        {
            return ValueTask.CompletedTask;
        }

        return this._mediator.Publish(new TwitchNewPaidSub(streamer: this.Streamer, user: user), cancellationToken: cancellationToken);
    }

    public ValueTask NewSubscriberPrimeAsync(Viewer user, in CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return ValueTask.CompletedTask;
        }

        this._stream.NewSubscriberPrime(user);

        if (this._options.IsSelf(user))
        {
            return ValueTask.CompletedTask;
        }

        return this._mediator.Publish(new TwitchNewPrimeSub(streamer: this.Streamer, user: user), cancellationToken: cancellationToken);
    }

    public ValueTask ResubscribePaidAsync(Viewer user, int months, in CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return ValueTask.CompletedTask;
        }

        this._stream.ResubscribePaid(user);

        if (this._options.IsSelf(user))
        {
            return ValueTask.CompletedTask;
        }

        return this._mediator.Publish(new TwitchPaidReSub(streamer: this.Streamer, user: user), cancellationToken: cancellationToken);
    }

    public ValueTask ResubscribePrimeAsync(Viewer user, int months, in CancellationToken cancellationToken)
    {
        if (this._stream == null)
        {
            return ValueTask.CompletedTask;
        }

        this._stream.ResubscribePrime(user);

        if (this._options.IsSelf(user))
        {
            return ValueTask.CompletedTask;
        }

        return this._mediator.Publish(new TwitchPrimeReSub(streamer: this.Streamer, user: user), cancellationToken: cancellationToken);
    }

    public async ValueTask NewFollowerAsync(Viewer user, CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{this.Streamer}: Followed by {user}");
        int followCount = await this._twitchStreamDataManager.RecordNewFollowerAsync(streamer: this.Streamer, username: user, cancellationToken: cancellationToken);

        TwitchUser? twitchUser = await this._userInfoService.GetUserAsync(userName: user, cancellationToken: cancellationToken);

        TwitchChannelNewFollower model;

        if (twitchUser != null)
        {
            model = new(Streamer: this.Streamer,
                        Viewer: user,
                        this._stream != null,
                        IsStreamer: twitchUser.IsStreamer,
                        AccountCreated: twitchUser.DateCreated,
                        FollowCount: followCount);
        }
        else
        {
            model = new(Streamer: this.Streamer, Viewer: user, this._stream != null, IsStreamer: false, AccountCreated: DateTimeOffset.MinValue, FollowCount: followCount);
        }

        await this._mediator.Publish(notification: model, cancellationToken: cancellationToken);
    }

    public ITwitchStreamSettings Settings => this._stream?.Settings ?? this._offlineStreamSettings;

    private ValueTask SaveStreamSettingsInternalAsync(ActiveStream stream, in CancellationToken cancellationToken)
    {
        StreamSettings newSettings = new(ChatWelcomesEnabled: stream.Settings.ChatWelcomesEnabled,
                                         RaidWelcomesEnabled: stream.Settings.RaidWelcomesEnabled,
                                         ThanksEnabled: stream.Settings.ThanksEnabled,
                                         AnnounceMilestonesEnabled: stream.Settings.AnnounceMilestonesEnabled,
                                         ShoutOutsEnabled: stream.Settings.ShoutOutsEnabled);

        this._logger.LogInformation($"{this.Streamer}: Settings changed, updating...");

        return this._twitchStreamDataManager.UpdateSettingsAsync(streamer: this.Streamer,
                                                                 streamStartDate: stream.StartedAt,
                                                                 settings: newSettings,
                                                                 cancellationToken: cancellationToken);
    }

    private async Task<bool> IsRegularChatterAsync(Streamer streamer, Viewer username, CancellationToken cancellationToken)
    {
        try
        {
            return await this._twitchStreamDataManager.IsRegularChatterAsync(streamer: streamer, username: username, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{streamer}: Is Regular Chatter: Failed to check {exception.Message}");

            return false;
        }
    }
}