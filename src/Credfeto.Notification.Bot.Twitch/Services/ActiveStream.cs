using System;
using System.Threading;
using NonBlocking;

namespace Credfeto.Notification.Bot.Twitch.Services;

internal sealed class ActiveStream
{
    private readonly ConcurrentDictionary<string, bool> _chatters;
    private readonly ConcurrentDictionary<string, bool> _raiders;

    private int _incidents;

    public ActiveStream(string gameName, in DateTime startedAt)
    {
        this.GameName = gameName;
        this.StartedAt = startedAt;

        this._incidents = 0;
        this._raiders = new(StringComparer.OrdinalIgnoreCase);
        this._chatters = new(StringComparer.OrdinalIgnoreCase);
    }

    public string GameName { get; }

    public DateTime StartedAt { get; }

    public void AddRaider(string raider)
    {
        if (this._raiders.TryAdd(key: raider, value: true))
        {
            // New Raider found
        }
    }

    public void AddChatter(string chatter)
    {
        if (this._chatters.TryAdd(key: chatter, value: true))
        {
            // New Chatter.
        }
    }

    public void AddIncident()
    {
        Interlocked.Increment(ref this._incidents);
    }
}