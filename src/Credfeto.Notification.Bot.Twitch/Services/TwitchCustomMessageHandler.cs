using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NonBlocking;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchCustomMessageHandler : ITwitchCustomMessageHandler
{
    private const RegexOptions REGEX_OPTIONS = RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.NonBacktracking |
                                               RegexOptions.CultureInvariant | RegexOptions.Singleline;

    private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5);

    private readonly ILogger<TwitchCustomMessageHandler> _logger;
    private readonly IMediator _mediator;
    private readonly ITwitchMessageTriggerDebounceFilter _twitchMessageTriggerDebounceFilter;
    private readonly ConcurrentDictionary<TwitchInputMessageMatch, TwitchOutputMessageMatch> _twitchMessageTriggers;

    public TwitchCustomMessageHandler(IOptions<TwitchBotOptions> options,
                                      IMediator mediator,
                                      ITwitchMessageTriggerDebounceFilter twitchMessageTriggerDebounceFilter,
                                      ILogger<TwitchCustomMessageHandler> logger)
    {
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this._twitchMessageTriggerDebounceFilter = twitchMessageTriggerDebounceFilter ?? throw new ArgumentNullException(nameof(twitchMessageTriggerDebounceFilter));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        TwitchBotOptions opts = (options ?? throw new ArgumentNullException(nameof(options))).Value;

        this._twitchMessageTriggers = BuildMessageTriggers(heists: opts.Heists, marbles: opts.Marbles);
    }

    public async Task<bool> HandleMessageAsync(TwitchIncomingMessage message, CancellationToken cancellationToken)
    {
        foreach ((TwitchInputMessageMatch trigger, TwitchOutputMessageMatch command) in this._twitchMessageTriggers)
        {
            if (!this.IsMatch(message: message, trigger: trigger))
            {
                continue;
            }

            return await this.SendMessageAsync(trigger: trigger, command: command, cancellationToken: cancellationToken);
        }

        return false;
    }

    private async Task<bool> SendMessageAsync(TwitchInputMessageMatch trigger, TwitchOutputMessageMatch command, CancellationToken cancellationToken)
    {
        if (!this._twitchMessageTriggerDebounceFilter.CanSend(command))
        {
            this._logger.LogInformation($"{trigger.Streamer}: Debouncing... {trigger.Chatter}: {trigger.Message}");

            // debounced.
            return true;
        }

        this._logger.LogInformation($"{trigger.Streamer}: Matched... {trigger.Chatter}: {trigger.Message}");
        await this._mediator.Publish(new CustomTriggeredMessage(streamer: trigger.Streamer, message: command.Message), cancellationToken: cancellationToken);

        return true;
    }

    private bool IsMatch(TwitchIncomingMessage message, TwitchInputMessageMatch trigger)
    {
        return trigger.Streamer == message.Streamer && trigger.Chatter == message.Chatter && trigger.MatchType switch
        {
            TwitchMessageMatchType.EXACT => IsExactMatch(message: message, trigger: trigger),
            TwitchMessageMatchType.CONTAINS => IsContainsMatch(message: message, trigger: trigger),
            TwitchMessageMatchType.STARTS_WITH => IsStartsWithMatch(message: message, trigger: trigger),
            TwitchMessageMatchType.ENDS_WITH => IsEndsWithMatch(message: message, trigger: trigger),
            TwitchMessageMatchType.REGEX => this.IsRegexMatch(message: message, trigger: trigger),
            _ => false
        };
    }

    private static bool IsContainsMatch(TwitchIncomingMessage message, TwitchInputMessageMatch trigger)
    {
        return message.Message.Contains(value: trigger.Message, comparisonType: StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool IsExactMatch(TwitchIncomingMessage message, TwitchInputMessageMatch trigger)
    {
        return StringComparer.InvariantCultureIgnoreCase.Equals(x: message.Message, y: trigger.Message);
    }

    private static bool IsStartsWithMatch(TwitchIncomingMessage message, TwitchInputMessageMatch trigger)
    {
        return message.Message.StartsWith(value: trigger.Message, comparisonType: StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool IsEndsWithMatch(TwitchIncomingMessage message, TwitchInputMessageMatch trigger)
    {
        return message.Message.EndsWith(value: trigger.Message, comparisonType: StringComparison.InvariantCultureIgnoreCase);
    }

    private bool IsRegexMatch(TwitchIncomingMessage message, TwitchInputMessageMatch trigger)
    {
        if (message.Message.StartsWith(value: "@credfeto", comparisonType: StringComparison.InvariantCultureIgnoreCase))
        {
            bool isMatch = IsRegexMatch(message: message.Message, pattern: trigger.Message);
            this._logger.LogInformation(
                $"{trigger.Streamer}: Checking match \"{trigger.Message}\" : Pattern: \"{trigger.Message}\" : Message: \"{message.Message}\" : Match: {isMatch}");

            return isMatch;
        }

        return IsRegexMatch(message: message.Message, pattern: trigger.Message);
    }

    private static bool IsRegexMatch(string message, string pattern)
    {
        return Regex.IsMatch(input: message, pattern: pattern, options: REGEX_OPTIONS, matchTimeout: RegexTimeout);
    }

    private static ConcurrentDictionary<TwitchInputMessageMatch, TwitchOutputMessageMatch> BuildMessageTriggers(IReadOnlyList<string> heists, IReadOnlyList<TwitchMarbles>? marbles)
    {
        ConcurrentDictionary<TwitchInputMessageMatch, TwitchOutputMessageMatch> triggers = new();

        Viewer streamLabs = Viewer.FromString("streamlabs");

        foreach (string streamer in heists)
        {
            Trace.WriteLine($"Adding heist trigger: {streamer}");

            TwitchInputMessageMatch trigger = new(Streamer.FromString(streamer),
                                                  chatter: streamLabs,
                                                  matchType: TwitchMessageMatchType.ENDS_WITH,
                                                  message: " is trying to get a crew together for a treasure hunt! Type !heist <amount> to join.");
            triggers.TryAdd(key: trigger, new(Streamer.FromString(streamer), message: "!heist all"));
        }

        if (marbles != null)
        {
            foreach (TwitchMarbles marble in marbles)
            {
                Trace.WriteLine($"Adding marbles trigger: {marble.Streamer}");
                TwitchInputMessageMatch trigger = new(Streamer.FromString(marble.Streamer),
                                                      Viewer.FromString(marble.Bot),
                                                      matchType: ConvertMatchType(marble.MatchType.ToUpperInvariant()),
                                                      message: marble.Match);

                triggers.TryAdd(key: trigger, new(Streamer.FromString(marble.Streamer), message: marble.Issue));
            }
        }

        return triggers;
    }

    private static TwitchMessageMatchType ConvertMatchType(string marbleMatchType)
    {
        return marbleMatchType switch
        {
            "EXACT" => TwitchMessageMatchType.EXACT,
            "CONTAINS" => TwitchMessageMatchType.CONTAINS,
            "STARTS_WITH" => TwitchMessageMatchType.STARTS_WITH,
            "ENDS_WITH" => TwitchMessageMatchType.ENDS_WITH,
            "REGEX" => TwitchMessageMatchType.REGEX,
            _ => throw new ArgumentOutOfRangeException(nameof(marbleMatchType), actualValue: marbleMatchType, message: null)
        };
    }
}