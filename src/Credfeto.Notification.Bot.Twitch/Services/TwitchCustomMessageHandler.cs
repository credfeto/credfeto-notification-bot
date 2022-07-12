using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NonBlocking;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchCustomMessageHandler : ITwitchCustomMessageHandler
{
    private readonly ILogger<TwitchCustomMessageHandler> _logger;
    private readonly IMediator _mediator;
    private readonly TwitchBotOptions _options;
    private readonly ITwitchMessageTriggerDebounceFilter _twitchMessageTriggerDebounceFilter;
    private readonly ConcurrentDictionary<TwitchMessageMatch, string> _twitchMessageTriggers;

    public TwitchCustomMessageHandler(IOptions<TwitchBotOptions> options,
                                      IMediator mediator,
                                      ITwitchMessageTriggerDebounceFilter twitchMessageTriggerDebounceFilter,
                                      ILogger<TwitchCustomMessageHandler> logger)
    {
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this._twitchMessageTriggerDebounceFilter = twitchMessageTriggerDebounceFilter ?? throw new ArgumentNullException(nameof(twitchMessageTriggerDebounceFilter));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

        this._twitchMessageTriggers = this.BuildMessageTriggers(heists: this._options.Heists, marbles: this._options.Marbles);
    }

    public async Task<bool> HandleMessageAsync(TwitchIncomingMessage message, CancellationToken cancellationToken)
    {
        foreach ((TwitchMessageMatch trigger, string command) in this._twitchMessageTriggers)
        {
            if (trigger.Streamer != message.Streamer)
            {
                continue;
            }

            if (trigger.Chatter != message.Chatter)
            {
                continue;
            }

            if (!StringComparer.InvariantCultureIgnoreCase.Equals(x: message.Message, y: command))
            {
                continue;
            }

            if (!this._twitchMessageTriggerDebounceFilter.CanSend(trigger))
            {
                this._logger.LogInformation($"{message.Streamer}: Debouncing... {message.Chatter}: {message.Message}");

                // debounced.
                return true;
            }

            this._logger.LogInformation($"{trigger.Streamer}: Matched... {trigger.Chatter}: {trigger.Message}");
            await this._mediator.Publish(new MarblesStarting(streamer: trigger.Streamer, message: command), cancellationToken: cancellationToken);

            return true;
        }

        return false;
    }

    private ConcurrentDictionary<TwitchMessageMatch, string> BuildMessageTriggers(List<string> heists, List<TwitchMarbles>? marbles)
    {
        ConcurrentDictionary<TwitchMessageMatch, string> triggers = new();

        // Viewer streamLabs = Viewer.FromString("streamlabs");
        foreach (string streamer in heists)
        {
            Trace.WriteLine($"Adding heist trigger: {streamer}");

            //     // TODO: Add EndsWith support
            //     // return StringComparer.InvariantCulture.Equals(x: e.ChatMessage.Username, y: "streamlabs") &&
            //     //        e.ChatMessage.Message.EndsWith(value: " is trying to get a crew together for a treasure hunt! Type !heist <amount> to join.", comparisonType: StringComparison.Ordinal);
            //
            //     TwitchMessageMatch trigger = new(Streamer.FromString(streamer), streamLabs, message: " is trying to get a crew together for a treasure hunt! Type !heist <amount> to join.");
            //     this._twitchMessageTriggers.TryAdd(key: trigger, value: "!heist all");
        }

        if (marbles != null)
        {
            foreach (TwitchMarbles marble in marbles)
            {
                Trace.WriteLine($"Adding marbles trigger: {marble.Streamer}");
                TwitchMessageMatch trigger = new(Streamer.FromString(marble.Streamer), Viewer.FromString(marble.Bot), message: marble.Match);
                this._twitchMessageTriggers.TryAdd(key: trigger, value: "!play");
            }
        }

        return triggers;
    }
}