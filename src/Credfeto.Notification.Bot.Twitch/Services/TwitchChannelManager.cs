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
    private readonly ConcurrentDictionary<string, TwitchChannelState> _streamStates;
    private readonly ITwitchStreamDataManager _twitchStreamDataManager;

    public TwitchChannelManager(IOptions<TwitchBotOptions> options,
                                IContributionThanks contributionThanks,
                                ITwitchStreamDataManager twitchStreamDataManager,
                                IMediator mediator,
                                ILogger<TwitchChannelManager> logger)
    {
        this._contributionThanks = contributionThanks ?? throw new ArgumentNullException(nameof(contributionThanks));
        this._twitchStreamDataManager = twitchStreamDataManager ?? throw new ArgumentNullException(nameof(twitchStreamDataManager));
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
                                                                  contributionThanks: this._contributionThanks,
                                                                  twitchStreamDataManager: this._twitchStreamDataManager,
                                                                  mediator: this._mediator,
                                                                  logger: this._logger));
    }
}