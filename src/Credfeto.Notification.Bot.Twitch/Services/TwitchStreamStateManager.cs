using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.StreamState;
using Microsoft.Extensions.Logging;
using NonBlocking;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchStreamStateManager : ITwitchStreamStateManager
{
    private readonly ConcurrentDictionary<Streamer, ITwitchChannelState> _channels = new();
    private readonly ILogger<TwitchStreamStateManager> _logger;

    public TwitchStreamStateManager(ILogger<TwitchStreamStateManager> logger)
    {
        this._logger = logger;
    }

    public ITwitchChannelState Get(Streamer streamer)
    {
        return this._channels.TryGetValue(key: streamer, out ITwitchChannelState? status)
            ? status
            : this._channels.GetOrAdd(key: streamer, new TwitchChannelState(streamerStreamer: streamer, logger: this._logger));
    }
}