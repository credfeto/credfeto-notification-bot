using System;
using Credfeto.Notification.Bot.Twitch.Models;
using Microsoft.Extensions.Logging;
using NonBlocking;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchChannelManager : ITwitchChannelManager
{
    private readonly ILogger<TwitchChannelManager> _logger;
    private readonly IRaidWelcome _raidWelcome;
    private readonly IShoutoutJoiner _shoutoutJoiner;
    private readonly ConcurrentDictionary<string, ChannelState> _streamStates;

    public TwitchChannelManager(IRaidWelcome raidWelcome, IShoutoutJoiner shoutoutJoiner, ILogger<TwitchChannelManager> logger)
    {
        this._raidWelcome = raidWelcome ?? throw new ArgumentNullException(nameof(raidWelcome));
        this._shoutoutJoiner = shoutoutJoiner ?? throw new ArgumentNullException(nameof(shoutoutJoiner));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._streamStates = new(comparer: StringComparer.OrdinalIgnoreCase);
    }

    /// <inheritdoc />
    public ChannelState GetChannel(string channel)
    {
        if (this._streamStates.TryGetValue(key: channel, out ChannelState? state))
        {
            return state;
        }

        return this._streamStates.GetOrAdd(key: channel, new ChannelState());
    }
}