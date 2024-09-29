using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Models;

[DebuggerDisplay("{Streamer.Value}: {Chatter.Value} -  {MatchType}: {Message}")]
public sealed class TwitchInputMessageMatch : IEquatable<TwitchInputMessageMatch>
{
    private const RegexOptions REGEX_OPTIONS = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.NonBacktracking | RegexOptions.CultureInvariant |
                                               RegexOptions.Singleline;

    private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5);

    private static readonly Dictionary<string, Regex> RegexCache = new(StringComparer.Ordinal);

    public TwitchInputMessageMatch(in Streamer streamer, in Viewer chatter, string message, TwitchMessageMatchType matchType)

    {
        this.Streamer = streamer;
        this.Chatter = chatter;
        this.Message = message;
        this.MatchType = matchType;
        this.Regex = BuildRegex(expression: message, matchType: matchType);
    }

    public Regex? Regex { get; }

    public Streamer Streamer { get; }

    public Viewer Chatter { get; }

    public string Message { get; }

    public TwitchMessageMatchType MatchType { get; }

    public bool Equals(TwitchInputMessageMatch? other)
    {
        if (ReferenceEquals(objA: null, objB: other))
        {
            return false;
        }

        if (ReferenceEquals(this, objB: other))
        {
            return true;
        }

        return this.Streamer.Equals(other.Streamer) && this.Chatter.Equals(other.Chatter) && StringComparer.Ordinal.Equals(x: this.Message, y: other.Message) && this.MatchType == other.MatchType;
    }

    private static Regex? BuildRegex(TwitchMessageMatchType matchType, string expression)
    {
        if (matchType == TwitchMessageMatchType.REGEX)
        {
            if (RegexCache.TryGetValue(key: expression, out Regex? regex))
            {
                return regex;
            }

            regex = new(pattern: expression, options: REGEX_OPTIONS, matchTimeout: RegexTimeout);
            RegexCache.Add(key: expression, value: regex);

            return regex;
        }

        return null;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, objB: obj) || obj is TwitchInputMessageMatch other && this.Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(value1: this.Streamer, value2: this.Chatter, value3: this.Message, value4: this.MatchType);
    }

    public static bool operator ==(TwitchInputMessageMatch? left, TwitchInputMessageMatch? right)
    {
        return Equals(objA: left, objB: right);
    }

    public static bool operator !=(TwitchInputMessageMatch? left, TwitchInputMessageMatch? right)
    {
        return !Equals(objA: left, objB: right);
    }
}