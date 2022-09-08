using System;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NonBlocking;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchStreamStatus : ITwitchStreamStatus, IDisposable
{
    private static readonly Regex StreamerUnknown = new(pattern: "No\\schannel\\swith\\sthen\\same\"\\s(?<Streamer>[\\w\\d]+)\"\\scould\\sbef\\sound.",
                                                        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
                                                        TimeSpan.FromSeconds(1));

    private readonly ConcurrentDictionary<Streamer, bool> _channels;

    private readonly SemaphoreSlim _lock;
    private readonly ILogger<TwitchStreamStatus> _logger;
    private readonly LiveStreamMonitorService _lsm;
    private readonly IMediator _mediator;
    private readonly IDisposable _offlineSubscription;
    private readonly IDisposable _onlineSubscription;
    private int _lastVersion;
    private int _version;

    public TwitchStreamStatus(IOptions<TwitchBotOptions> options, IMediator mediator, ILogger<TwitchStreamStatus> logger)
    {
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._version = 0;
        this._lastVersion = 0;
        this._channels = new();
        this._lock = new(1);

        this._lsm = new(options.Value.ConfigureTwitchApi());

        this._onlineSubscription = Observable.FromEventPattern<OnStreamOnlineArgs>(addHandler: h => this._lsm.OnStreamOnline += h, removeHandler: h => this._lsm.OnStreamOnline -= h)
                                             .Select(messageEvent => messageEvent.EventArgs)
                                             .Select(e => Observable.FromAsync(cancellationToken => this.OnStreamOnlineAsync(e: e, cancellationToken: cancellationToken)))
                                             .Concat()
                                             .Subscribe();

        this._offlineSubscription = Observable.FromEventPattern<OnStreamOfflineArgs>(addHandler: h => this._lsm.OnStreamOffline += h, removeHandler: h => this._lsm.OnStreamOffline -= h)
                                              .Select(messageEvent => messageEvent.EventArgs)
                                              .Select(e => Observable.FromAsync(cancellationToken => this.OnStreamOfflineAsync(e: e, cancellationToken: cancellationToken)))
                                              .Concat()
                                              .Subscribe();
    }

    public void Dispose()
    {
        this._offlineSubscription.Dispose();
        this._onlineSubscription.Dispose();
        this._lock.Dispose();
    }

    public async Task UpdateAsync()
    {
        if (this._channels.IsEmpty)
        {
            return;
        }

        await this._lock.WaitAsync();

        try
        {
            if (this._lastVersion != this._version)
            {
                this._lsm.SetChannelsByName(this._channels.Keys.Select(c => c.Value)
                                                .ToList());
                this._lastVersion = this._version;
            }

            await this._lsm.UpdateLiveStreamersAsync();
        }
        catch (InvalidOperationException exception)
        {
            Match match = StreamerUnknown.Match(input: exception.Message);

            if (!match.Success)
            {
                // Another error
                throw;
            }

            Streamer streamer = Streamer.FromString(match.Groups["Streamer"]
                                                         .Value);
            this._logger.LogError(new(exception.HResult), exception: exception, $"Streamer {streamer.Value} not found");

            this._channels.TryRemove(key: streamer, value: out _);
        }
        finally
        {
            this._lock.Release();
        }
    }

    public async Task EnableAsync(Streamer streamer)
    {
        if (this._channels.TryAdd(key: streamer, value: true))
        {
            Interlocked.Increment(location: ref this._version);

            await this.UpdateAsync();
        }
    }

    private async Task OnStreamOnlineAsync(OnStreamOnlineArgs e, CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.Channel);
        this._logger.LogWarning($"{streamer}: Started streaming \"{e.Stream.Title}\" ({e.Stream.GameName}) at {e.Stream.StartedAt}");

        try
        {
            await this._mediator.Publish(new TwitchStreamOnline(streamer: streamer, title: e.Stream.Title, gameName: e.Stream.GameName, startedAt: e.Stream.StartedAt),
                                         cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{e.Channel}: Failed to notify Started streaming");
        }
    }

    private async Task OnStreamOfflineAsync(OnStreamOfflineArgs e, CancellationToken cancellationToken)
    {
        Streamer streamer = Streamer.FromString(e.Channel);

        try
        {
            await this._mediator.Publish(new TwitchStreamOffline(streamer: streamer, title: e.Stream.Title, gameName: e.Stream.GameName, startedAt: e.Stream.StartedAt),
                                         cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"{e.Channel}: Failed to notify Stopped streaming");
        }
    }
}