using System;
using System.Collections.Immutable;
using System.Linq;
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
    private readonly ImmutableHashSet<Streamer> _allowedHosts;
    private readonly Streamer _bot;
    private readonly ILogger<Hoster> _logger;
    private readonly ConcurrentDictionary<Streamer, DateTime> _streamers;
    private readonly ITwitchChat _twitchChat;
    private Streamer? _hosting;

    public Hoster(IOptions<TwitchBotOptions> options, ITwitchChat twitchChat, IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ILogger<Hoster> logger)
        : base(twitchChatMessageChannel)
    {
        this._twitchChat = twitchChat ?? throw new ArgumentNullException(nameof(twitchChat));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._streamers = new();
        this._hosting = null;
        this._bot = Streamer.FromString((options ?? throw new ArgumentNullException(nameof(options))).Value.Authentication.UserName);
        this._allowedHosts = options.Value.Channels.Select(c => Streamer.FromString(c.ChannelName))
                                    .Where(c => c != this._bot)
                                    .Distinct()
                                    .ToImmutableHashSet();
    }

    public async Task StreamOnlineAsync(Streamer streamer, DateTime streamStartTime, CancellationToken cancellationToken)
    {
        if (!this._allowedHosts.Contains(streamer))
        {
            this._logger.LogWarning($"{streamer}: Not permitted to host");

            return;
        }

        this._logger.LogInformation($"{streamer}: hosting for stream [Online]");

        if (!this._streamers.TryAdd(key: streamer, value: streamStartTime))
        {
            this._twitchChat.JoinChat(streamer);
            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken: cancellationToken);
        }

        await this.UpdateHostingAsync(cancellationToken);
    }

    public Task StreamOfflineAsync(Streamer streamer, CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{streamer}: hosting for stream [Offline]");

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
        await this.SendMessageAsync(streamer: this._bot, priority: MessagePriority.SLOW, $"/host @{active}", cancellationToken: cancellationToken);

        this._hosting = active;

        return active;
    }
}