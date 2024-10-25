using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Services.LoggingExtensions;
using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchCustomMessageHandler : ITwitchCustomMessageHandler
{
    private readonly Viewer _chatUser;

    private readonly ILogger<TwitchCustomMessageHandler> _logger;
    private readonly IMediator _mediator;
    private readonly ITwitchMessageTriggerDebounceFilter _twitchMessageTriggerDebounceFilter;
    private readonly Dictionary<Streamer, Dictionary<TwitchInputMessageMatch, TwitchOutputMessageMatch>> _twitchMessageTriggers;

    public TwitchCustomMessageHandler(IOptions<TwitchBotOptions> options,
                                      IMediator mediator,
                                      ITwitchMessageTriggerDebounceFilter twitchMessageTriggerDebounceFilter,
                                      ILogger<TwitchCustomMessageHandler> logger)
    {
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this._twitchMessageTriggerDebounceFilter = twitchMessageTriggerDebounceFilter ?? throw new ArgumentNullException(nameof(twitchMessageTriggerDebounceFilter));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        TwitchBotOptions opts = (options ?? throw new ArgumentNullException(nameof(options))).Value;

        this._chatUser = Viewer.FromString(opts.Authentication.Chat.UserName);

        this._twitchMessageTriggers = BuildMessageTriggers(twitchChatCommands: opts.ChatCommands, logger: logger);
    }

    public async Task<bool> HandleMessageAsync(TwitchIncomingMessage message, CancellationToken cancellationToken)
    {
        if (!this._twitchMessageTriggers.TryGetValue(key: message.Streamer, out Dictionary<TwitchInputMessageMatch, TwitchOutputMessageMatch>? triggers))
        {
            return false;
        }

        foreach ((TwitchInputMessageMatch trigger, TwitchOutputMessageMatch command) in triggers)
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
            this._logger.Debouncing(streamer: trigger.Streamer, chatter: trigger.Chatter, message: trigger.Message);

            // debounced.
            return true;
        }

        this._logger.Matched(streamer: trigger.Streamer, chatter: trigger.Chatter, message: trigger.Message);
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
        if (trigger.Regex is null)
        {
            return false;
        }

        bool isMatch = trigger.Regex.IsMatch(message.Message);

        if (this.IsDirectedAtBot(message))
        {
            this._logger.CheckingMatch(streamer: trigger.Streamer, chatter: trigger.Chatter, triggerMessage: trigger.Message, sendMessage: message.Message, isMatch: isMatch);
        }

        return isMatch;
    }

    private bool IsDirectedAtBot(TwitchIncomingMessage message)
    {
        return message.Message.StartsWith("@" + this._chatUser.Value, comparisonType: StringComparison.InvariantCultureIgnoreCase);
    }

    private static Dictionary<Streamer, Dictionary<TwitchInputMessageMatch, TwitchOutputMessageMatch>> BuildMessageTriggers(
        IReadOnlyList<TwitchChatCommand> twitchChatCommands,
        ILogger<TwitchCustomMessageHandler> logger)
    {
        Dictionary<Streamer, Dictionary<TwitchInputMessageMatch, TwitchOutputMessageMatch>> streamerTriggers = [];

        foreach (TwitchChatCommand twitchChatCommand in twitchChatCommands)
        {
            Streamer streamer = Streamer.FromString(twitchChatCommand.Streamer);

            if (!streamerTriggers.TryGetValue(key: streamer, out Dictionary<TwitchInputMessageMatch, TwitchOutputMessageMatch>? triggers))
            {
                triggers = [];
                streamerTriggers.Add(key: streamer, value: triggers);
            }

            Viewer viewer = Viewer.FromString(twitchChatCommand.Bot);
            logger.AddingChatCommandTrigger(streamer: streamer, chatter: viewer, triggerMessage: twitchChatCommand.Match);
            TwitchInputMessageMatch trigger = new(streamer: streamer, chatter: viewer, message: twitchChatCommand.Match, ConvertMatchType(twitchChatCommand.MatchType.ToUpperInvariant()));
            TwitchOutputMessageMatch response = new(streamer: streamer, message: twitchChatCommand.Issue);

            triggers.TryAdd(key: trigger, value: response);
        }

        return streamerTriggers;
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