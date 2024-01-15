using System;
using System.Diagnostics;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Models;

[DebuggerDisplay("{Streamer.Value}: {Message}")]
public sealed class TwitchOutputMessageMatch : IEquatable<TwitchOutputMessageMatch>
{
    public TwitchOutputMessageMatch(in Streamer streamer, string message)

    {
        this.Streamer = streamer;
        this.Message = message;
    }

    public Streamer Streamer { get; }

    public string Message { get; }

    public bool Equals(TwitchOutputMessageMatch? other)
    {
        if (ReferenceEquals(objA: null, objB: other))
        {
            return false;
        }

        if (ReferenceEquals(this, objB: other))
        {
            return true;
        }

        return this.Streamer.Equals(other.Streamer) && StringComparer.Ordinal.Equals(x: this.Message, y: other.Message);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, objB: obj) || obj is TwitchInputMessageMatch other && this.Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(value1: this.Streamer, value2: this.Message);
    }

    public static bool operator ==(TwitchOutputMessageMatch? left, TwitchOutputMessageMatch? right)
    {
        return Equals(objA: left, objB: right);
    }

    public static bool operator !=(TwitchOutputMessageMatch? left, TwitchOutputMessageMatch? right)
    {
        return !Equals(objA: left, objB: right);
    }
}