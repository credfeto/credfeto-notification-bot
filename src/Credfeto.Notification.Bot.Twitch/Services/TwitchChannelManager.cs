using System;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NonBlocking;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchChannelManager : ITwitchChannelManager
{
    private readonly IChannelFollowCount _channelFollowCount;
    private readonly IContributionThanks _contributionThanks;
    private readonly IFollowerMilestone _followerMilestone;
    private readonly ILogger<TwitchChannelManager> _logger;
    private readonly TwitchBotOptions _options;
    private readonly IRaidWelcome _raidWelcome;
    private readonly IShoutoutJoiner _shoutoutJoiner;
    private readonly ConcurrentDictionary<string, TwitchChannelState> _streamStates;
    private readonly ITwitchStreamDataManager _twitchStreamDataManager;
    private readonly IUserInfoService _userInfoService;
    private readonly IWelcomeWaggon _welcomeWaggon;

    public TwitchChannelManager(IOptions<TwitchBotOptions> options,
                                IWelcomeWaggon welcomeWaggon,
                                IRaidWelcome raidWelcome,
                                IShoutoutJoiner shoutoutJoiner,
                                IContributionThanks contributionThanks,
                                ITwitchStreamDataManager twitchStreamDataManager,
                                IUserInfoService userInfoService,
                                IChannelFollowCount channelFollowCount,
                                IFollowerMilestone followerMilestone,
                                ILogger<TwitchChannelManager> logger)
    {
        this._welcomeWaggon = welcomeWaggon ?? throw new ArgumentNullException(nameof(welcomeWaggon));
        this._raidWelcome = raidWelcome ?? throw new ArgumentNullException(nameof(raidWelcome));
        this._shoutoutJoiner = shoutoutJoiner ?? throw new ArgumentNullException(nameof(shoutoutJoiner));
        this._contributionThanks = contributionThanks ?? throw new ArgumentNullException(nameof(contributionThanks));
        this._twitchStreamDataManager = twitchStreamDataManager ?? throw new ArgumentNullException(nameof(twitchStreamDataManager));
        this._userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        this._channelFollowCount = channelFollowCount ?? throw new ArgumentNullException(nameof(channelFollowCount));
        this._followerMilestone = followerMilestone ?? throw new ArgumentNullException(nameof(followerMilestone));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._streamStates = new(comparer: StringComparer.OrdinalIgnoreCase);
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
    }

    /// <inheritdoc />
    public TwitchChannelState GetChannel(string channel)
    {
        if (this._streamStates.TryGetValue(key: channel, out TwitchChannelState? state))
        {
            return state;
        }

        return this._streamStates.GetOrAdd(key: channel,
                                           new TwitchChannelState(channel.ToLowerInvariant(),
                                                                  options: this._options,
                                                                  raidWelcome: this._raidWelcome,
                                                                  shoutoutJoiner: this._shoutoutJoiner,
                                                                  contributionThanks: this._contributionThanks,
                                                                  twitchStreamDataManager: this._twitchStreamDataManager,
                                                                  userInfoService: this._userInfoService,
                                                                  channelFollowCount: this._channelFollowCount,
                                                                  followerMilestone: this._followerMilestone,
                                                                  welcomeWaggon: this._welcomeWaggon,
                                                                  logger: this._logger));
    }
}