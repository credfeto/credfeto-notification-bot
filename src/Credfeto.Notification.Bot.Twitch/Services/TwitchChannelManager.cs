using System;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.StreamState;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NonBlocking;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchChannelManager : ITwitchChannelManager
{
    private readonly ILogger<TwitchChannelManager> _logger;
    private readonly IMediator _mediator;
    private readonly TwitchBotOptions _options;
    private readonly ConcurrentDictionary<Streamer, TwitchChannelState> _streamStates;
    private readonly ITwitchStreamDataManager _twitchStreamDataManager;
    private readonly IUserInfoService _userInfoService;

    public TwitchChannelManager(IOptions<TwitchBotOptions> options,
                                IUserInfoService userInfoService,
                                ITwitchStreamDataManager twitchStreamDataManager,
                                IMediator mediator,
                                ILogger<TwitchChannelManager> logger)
    {
        this._userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        this._twitchStreamDataManager = twitchStreamDataManager ?? throw new ArgumentNullException(nameof(twitchStreamDataManager));
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._streamStates = new();
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
    }

    public ITwitchChannelState GetStreamer(Streamer streamer)
    {
        if (!this._options.IsModChannel(streamer))
        {
            throw new ArgumentOutOfRangeException(nameof(streamer), actualValue: streamer, message: "The streamer is not a mod channel");
        }

        if (this._streamStates.TryGetValue(key: streamer, out TwitchChannelState? state))
        {
            return state;
        }

        return this._streamStates.GetOrAdd(key: streamer,
                                           new TwitchChannelState(streamerStreamer: streamer,
                                                                  options: this._options,
                                                                  userInfoService: this._userInfoService,
                                                                  twitchStreamDataManager: this._twitchStreamDataManager,
                                                                  mediator: this._mediator,
                                                                  logger: this._logger));
    }
}