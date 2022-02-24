using System;

namespace Credfeto.Notification.Bot.Twitch.Services;

internal sealed class StreamState
{
    private ActiveStream? _stream;

    public void Online(string gameName, in DateTime startDate)
    {
        // TODO: Implement
        this._stream = new(gameName: gameName, startedAt: startDate);
    }

    public void Offline()
    {
        // TODO: Implement
        this._stream = null;
    }

    public void ClearChat()
    {
        // TODO: Implement
        this._stream?.AddIncident();
    }

    public void Raided(string raider)
    {
        // TODO: Implement
        this._stream?.AddRaider(raider);
    }

    public bool ChatMessage(string user, string message, int bits)
    {
        // TODO: Implement
        return this._stream?.AddChatter(user) == true;
    }

    public void GiftedMultiple(string giftedBy, int count, string months)
    {
        this._stream?.GiftedSub(giftedBy: giftedBy, count: count);
    }

    public void GiftedSub(string giftedBy, string months)
    {
        this._stream?.GiftedSub(giftedBy: giftedBy, count: 1);
    }

    public void ContinuedSub(string user)
    {
        this._stream?.ContinuedSub(user);
    }

    public void PrimeToPaid(string user)
    {
        this._stream?.PrimeToPaid(user);
    }

    public void NewSubscriberPaid(string user)
    {
        this._stream?.NewSubscriberPaid(user);
    }

    public void NewSubscriberPrime(string user)
    {
        this._stream?.NewSubscriberPrime(user);
    }

    public void ResubscribePaid(string user, int months)
    {
        this._stream?.ResubscribePaid(user);
    }

    public void ResubscribePrime(string user, int months)
    {
        this._stream?.ResubscribePrime(user);
    }
}