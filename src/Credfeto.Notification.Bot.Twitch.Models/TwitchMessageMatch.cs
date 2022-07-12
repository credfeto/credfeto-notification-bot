using System;
using System.Diagnostics;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Models;

[DebuggerDisplay("{Streamer.Value}\\{Chatter.Value}: {Message}")]
public sealed class TwitchMessageMatch : IEquatable<TwitchMessageMatch>
{
    public TwitchMessageMatch(in Streamer streamer, in Viewer chatter, string message)
    {
        this.Streamer = streamer;
        this.Chatter = chatter;
        this.Message = message;
    }

    public Streamer Streamer { get; }

    public Viewer Chatter { get; }

    public string Message { get; }

    public bool Equals(TwitchMessageMatch? other)
    {
        if (ReferenceEquals(objA: null, objB: other))
        {
            return false;
        }

        if (ReferenceEquals(this, objB: other))
        {
            return true;
        }

        return this.Streamer.Equals(other.Streamer) && this.Chatter.Equals(other.Chatter) && this.Message == other.Message;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, objB: obj) || obj is TwitchMessageMatch other && this.Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(value1: this.Streamer, value2: this.Chatter, value3: this.Message);
    }

    public static bool operator ==(TwitchMessageMatch? left, TwitchMessageMatch? right)
    {
        return Equals(objA: left, objB: right);
    }

    public static bool operator !=(TwitchMessageMatch? left, TwitchMessageMatch? right)
    {
        return !Equals(objA: left, objB: right);
    }
}