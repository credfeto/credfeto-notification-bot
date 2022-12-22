using System;
using Credfeto.Date.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Microsoft.Extensions.Logging;
using NonBlocking;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchMessageTriggerDebounceFilter : ITwitchMessageTriggerDebounceFilter
{
    private static readonly TimeSpan MinIntervalBetweenMatches = TimeSpan.FromMinutes(1);
    private readonly ICurrentTimeSource _currentTimeSource;
    private readonly ConcurrentDictionary<TwitchOutputMessageMatch, When> _debounce;
    private readonly ILogger<TwitchMessageTriggerDebounceFilter> _logger;

    public TwitchMessageTriggerDebounceFilter(ICurrentTimeSource currentTimeSource, ILogger<TwitchMessageTriggerDebounceFilter> logger)
    {
        this._currentTimeSource = currentTimeSource ?? throw new ArgumentNullException(nameof(currentTimeSource));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._debounce = new();
    }

    public bool CanSend(TwitchOutputMessageMatch match)
    {
        When when = this.GetMatch(match);

        DateTimeOffset now = this._currentTimeSource.UtcNow();

        bool canSend = when.CanSend(now: now, minIntervalBetweenMatches: MinIntervalBetweenMatches);

        this._logger.LogDebug($"{match.Streamer}\\{match.Message}: {canSend}");

        return canSend;
    }

    private When GetMatch(TwitchOutputMessageMatch match)
    {
        if (this._debounce.TryGetValue(key: match, out When? when))
        {
            return when;
        }

        return this._debounce.GetOrAdd(key: match, new When(lastMatch: DateTimeOffset.MinValue));
    }

    private sealed class When
    {
        private DateTimeOffset _lastMatch;

        public When(in DateTimeOffset lastMatch)
        {
            this._lastMatch = lastMatch;
        }

        public bool CanSend(in DateTimeOffset now, in TimeSpan minIntervalBetweenMatches)
        {
            TimeSpan timeSinceLastMatch = now - this._lastMatch;

            if (now > this._lastMatch)
            {
                // Always move forwards in time... in case tracking item is being spammed
                this._lastMatch = now;
            }

            if (timeSinceLastMatch > minIntervalBetweenMatches)
            {
                return true;
            }

            return false;
        }
    }
}