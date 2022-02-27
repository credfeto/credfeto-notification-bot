using System;
using Credfeto.Notification.Bot.Twitch.Models;
using NonBlocking;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchChannelManager : ITwitchChannelManager
{
    private readonly ConcurrentDictionary<string, ChannelState> _streamStates;

    public TwitchChannelManager()
    {
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