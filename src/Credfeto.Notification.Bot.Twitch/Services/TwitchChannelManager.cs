using System;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NonBlocking;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchChannelManager : ITwitchChannelManager
{
    private readonly IContributionThanks _contributionThanks;
    private readonly ILogger<TwitchChannelManager> _logger;
    private readonly IMediator _mediator;
    private readonly TwitchBotOptions _options;
    private readonly IShoutoutJoiner _shoutoutJoiner;
    private readonly ConcurrentDictionary<string, TwitchChannelState> _streamStates;
    private readonly ITwitchStreamDataManager _twitchStreamDataManager;
    private readonly IUserInfoService _userInfoService;
    private readonly IWelcomeWaggon _welcomeWaggon;

    public TwitchChannelManager(IOptions<TwitchBotOptions> options,
                                IWelcomeWaggon welcomeWaggon,
                                IShoutoutJoiner shoutoutJoiner,
                                IContributionThanks contributionThanks,
                                ITwitchStreamDataManager twitchStreamDataManager,
                                IUserInfoService userInfoService,
                                IMediator mediator,
                                ILogger<TwitchChannelManager> logger)
    {
        this._welcomeWaggon = welcomeWaggon ?? throw new ArgumentNullException(nameof(welcomeWaggon));
        this._shoutoutJoiner = shoutoutJoiner ?? throw new ArgumentNullException(nameof(shoutoutJoiner));
        this._contributionThanks = contributionThanks ?? throw new ArgumentNullException(nameof(contributionThanks));
        this._twitchStreamDataManager = twitchStreamDataManager ?? throw new ArgumentNullException(nameof(twitchStreamDataManager));
        this._userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
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
                                                                  shoutoutJoiner: this._shoutoutJoiner,
                                                                  contributionThanks: this._contributionThanks,
                                                                  twitchStreamDataManager: this._twitchStreamDataManager,
                                                                  userInfoService: this._userInfoService,
                                                                  welcomeWaggon: this._welcomeWaggon,
                                                                  mediator: this._mediator,
                                                                  logger: this._logger));
    }
}