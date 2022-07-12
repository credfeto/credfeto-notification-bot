using System;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Models;
using Microsoft.Extensions.Logging;
using NonBlocking;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchMessageTriggerDebounceFilter : ITwitchMessageTriggerDebounceFilter
{
    private static readonly TimeSpan MinIntervalBetweenMatches = TimeSpan.FromMinutes(1);
    private readonly ICurrentTimeSource _currentTimeSource;
    private readonly ConcurrentDictionary<TwitchMessageMatch, When> _debounce;
    private readonly ILogger<TwitchMessageTriggerDebounceFilter> _logger;

    public TwitchMessageTriggerDebounceFilter(ICurrentTimeSource currentTimeSource, ILogger<TwitchMessageTriggerDebounceFilter> logger)
    {
        this._currentTimeSource = currentTimeSource ?? throw new ArgumentNullException(nameof(currentTimeSource));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._debounce = new();
    }

    public bool CanSend(TwitchMessageMatch match)
    {
        When when = this.GetMatch(match);

        DateTime now = this._currentTimeSource.UtcNow();

        bool canSend = when.CanSend(now: now, minIntervalBetweenMatches: MinIntervalBetweenMatches);

        this._logger.LogDebug($"{match.Streamer}\\{match.Chatter}: {canSend}");

        return canSend;
    }

    private When GetMatch(TwitchMessageMatch match)
    {
        if (this._debounce.TryGetValue(key: match, out When? when))
        {
            return when;
        }

        return this._debounce.GetOrAdd(key: match, new When(lastMatch: DateTime.MinValue));
    }

    private sealed class When
    {
        private DateTime _lastMatch;

        public When(in DateTime lastMatch)
        {
            this._lastMatch = lastMatch;
        }

        public bool CanSend(in DateTime now, in TimeSpan minIntervalBetweenMatches)
        {
            TimeSpan timeSinceLastMatch = now - this._lastMatch;

            if (timeSinceLastMatch > minIntervalBetweenMatches && now > this._lastMatch)
            {
                this._lastMatch = now;

                return true;
            }

            return false;
        }
    }
}