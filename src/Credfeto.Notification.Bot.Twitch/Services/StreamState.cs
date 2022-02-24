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

    public void ChatMessage(string user, string message, int bits)
    {
        // TODO: Implement
        this._stream?.AddChatter(user);
    }
}