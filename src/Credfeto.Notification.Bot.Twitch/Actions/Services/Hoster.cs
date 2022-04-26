using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.StreamState;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NonBlocking;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class Hoster : MessageSenderBase, IHoster
{
    private readonly Streamer _bot;
    private readonly ILogger<Hoster> _logger;
    private readonly ConcurrentDictionary<Streamer, DateTime> _streamers;
    private Streamer? _hosting;

    public Hoster(IOptions<TwitchBotOptions> options, IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ILogger<Hoster> logger)
        : base(twitchChatMessageChannel)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._streamers = new();
        this._hosting = null;
        this._bot = Streamer.FromString((options ?? throw new ArgumentNullException(nameof(options))).Value.Authentication.UserName);
    }

    public Task StreamOnlineAsync(Streamer streamer, DateTime streamStartTime, CancellationToken cancellationToken)
    {
        if (!this._streamers.TryAdd(key: streamer, value: streamStartTime))
        {
            // already streaming
            return Task.CompletedTask;
        }

        return this.UpdateHostingAsync(cancellationToken);
    }

    public Task StreamOfflineAsync(Streamer streamer, CancellationToken cancellationToken)
    {
        this._streamers.TryRemove(key: streamer, value: out _);

        return this.UpdateHostingAsync(cancellationToken);
    }

    private async Task UpdateHostingAsync(CancellationToken cancellationToken)
    {
        Streamer? active = null;
        DateTime earliestStartTime = DateTime.MaxValue;

        foreach ((Streamer candidateStreamer, DateTime streamStarted) in this._streamers)
        {
            if (streamStarted < earliestStartTime)
            {
                active = candidateStreamer;
                earliestStartTime = streamStarted;
            }
        }

        if (active != null)
        {
            this._hosting = await this.HostStreamAsync(active: active.Value, cancellationToken: cancellationToken);
        }
    }

    private async Task<Streamer> HostStreamAsync(Streamer active, CancellationToken cancellationToken)
    {
        if (this._hosting != null)
        {
            if (active == this._hosting.Value)
            {
                return active;
            }
        }

        // TODO: Issue Host Stream Command
        this._logger.LogWarning($"Requesting host of {active}");
        await this.SendMessageAsync(streamer: this._bot, $"/host {active}", cancellationToken: cancellationToken);

        this._hosting = active;

        return active;
    }
}