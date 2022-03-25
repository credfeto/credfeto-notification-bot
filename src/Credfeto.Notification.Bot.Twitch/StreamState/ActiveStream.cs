using System;
using System.Diagnostics;
using System.Threading;
using NonBlocking;

namespace Credfeto.Notification.Bot.Twitch.StreamState;

[DebuggerDisplay("{GameName}: Started: {StartedAt}")]
internal sealed class ActiveStream
{
    private readonly ConcurrentDictionary<string, bool> _bitGifters;
    private readonly ConcurrentDictionary<string, bool> _chatters;
    private readonly ConcurrentDictionary<string, bool> _followers;
    private readonly ConcurrentDictionary<string, bool> _raiders;
    private readonly ConcurrentDictionary<string, bool> _subGifters;
    private readonly ConcurrentDictionary<string, bool> _subscribers;

    private int _incidents;

    public ActiveStream(string gameName, in DateTime startedAt)
    {
        this.GameName = gameName;
        this.StartedAt = startedAt;

        this._incidents = 0;
        this._raiders = new(StringComparer.OrdinalIgnoreCase);
        this._chatters = new(StringComparer.OrdinalIgnoreCase);
        this._subscribers = new(StringComparer.OrdinalIgnoreCase);
        this._subGifters = new(StringComparer.OrdinalIgnoreCase);
        this._bitGifters = new(StringComparer.OrdinalIgnoreCase);
        this._followers = new(StringComparer.OrdinalIgnoreCase);
    }

    public string GameName { get; }

    public DateTime StartedAt { get; }

    public bool AddRaider(string raider, int viewerCount)
    {
        if (this._raiders.TryAdd(key: raider, value: true))
        {
            // New Raider found
            return true;
        }

        return false;
    }

    public void AddBitGifter(string user, int bits)
    {
        if (this._bitGifters.TryAdd(key: user, value: true))
        {
            // New Raider found
        }
    }

    public bool AddChatter(string chatter)
    {
        if (this._chatters.TryAdd(key: chatter, value: true))
        {
            // New Chatter.
            return true;
        }

        return false;
    }

    public void AddIncident()
    {
        Interlocked.Increment(ref this._incidents);
    }

    public void GiftedSub(string giftedBy, int count)
    {
        if (this._subGifters.TryAdd(key: giftedBy, value: true))
        {
            // New Gifter
        }
    }

    public void ContinuedSub(string user)
    {
        if (this._subscribers.TryAdd(key: user, value: true))
        {
            // Continued sub gifted by someone else - not really a new subscriber
        }
    }

    public void PrimeToPaid(string user)
    {
        if (this._subscribers.TryAdd(key: user, value: true))
        {
            // Converted prime to paid - not really a new subscriber
        }
    }

    public void NewSubscriberPaid(string user)
    {
        if (this._subscribers.TryAdd(key: user, value: true))
        {
            // New sub
        }
    }

    public void NewSubscriberPrime(string user)
    {
        if (this._subscribers.TryAdd(key: user, value: true))
        {
            // New sub
        }
    }

    public void ResubscribePaid(string user)
    {
        if (this._subscribers.TryAdd(key: user, value: true))
        {
            // Resub
        }
    }

    public void ResubscribePrime(string user)
    {
        if (this._subscribers.TryAdd(key: user, value: true))
        {
            // Resub
        }
    }

    public bool Follow(string user)
    {
        if (this._followers.TryAdd(key: user, value: true))
        {
            // New Follow
            return true;
        }

        return false;
    }
}