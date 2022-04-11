using System;
using System.Diagnostics;
using System.Threading;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using NonBlocking;

namespace Credfeto.Notification.Bot.Twitch.StreamState;

[DebuggerDisplay("{GameName}: Started: {StartedAt}")]
internal sealed class ActiveStream
{
    private readonly ConcurrentDictionary<Viewer, bool> _bitGifters;
    private readonly ConcurrentDictionary<Viewer, bool> _chatters;
    private readonly ConcurrentDictionary<Viewer, bool> _followers;
    private readonly ConcurrentDictionary<Viewer, bool> _raiders;
    private readonly ConcurrentDictionary<Viewer, bool> _subGifters;
    private readonly ConcurrentDictionary<Viewer, bool> _subscribers;

    private int _incidents;

    public ActiveStream(string gameName, in DateTime startedAt)
    {
        this.GameName = gameName;
        this.StartedAt = startedAt;

        this._incidents = 0;
        this._raiders = new();
        this._chatters = new();
        this._subscribers = new();
        this._subGifters = new();
        this._bitGifters = new();
        this._followers = new();
    }

    public string GameName { get; }

    public DateTime StartedAt { get; }

    public bool AddRaider(in Viewer raider, int viewerCount)
    {
        if (this._raiders.TryAdd(key: raider, value: true))
        {
            // New Raider found
            return true;
        }

        return false;
    }

    public void AddBitGifter(in Viewer user, int bits)
    {
        if (this._bitGifters.TryAdd(key: user, value: true))
        {
            // New Raider found
        }
    }

    public bool AddChatter(in Viewer chatter)
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

    public void GiftedSub(in Viewer giftedBy, int count)
    {
        if (this._subGifters.TryAdd(key: giftedBy, value: true))
        {
            // New Gifter
        }
    }

    public void ContinuedSub(in Viewer user)
    {
        if (this._subscribers.TryAdd(key: user, value: true))
        {
            // Continued sub gifted by someone else - not really a new subscriber
        }
    }

    public void PrimeToPaid(in Viewer user)
    {
        if (this._subscribers.TryAdd(key: user, value: true))
        {
            // Converted prime to paid - not really a new subscriber
        }
    }

    public void NewSubscriberPaid(in Viewer user)
    {
        if (this._subscribers.TryAdd(key: user, value: true))
        {
            // New sub
        }
    }

    public void NewSubscriberPrime(in Viewer user)
    {
        if (this._subscribers.TryAdd(key: user, value: true))
        {
            // New sub
        }
    }

    public void ResubscribePaid(in Viewer user)
    {
        if (this._subscribers.TryAdd(key: user, value: true))
        {
            // Resub
        }
    }

    public void ResubscribePrime(in Viewer user)
    {
        if (this._subscribers.TryAdd(key: user, value: true))
        {
            // Resub
        }
    }

    public bool Follow(in Viewer user)
    {
        if (this._followers.TryAdd(key: user, value: true))
        {
            // New Follow
            return true;
        }

        return false;
    }
}