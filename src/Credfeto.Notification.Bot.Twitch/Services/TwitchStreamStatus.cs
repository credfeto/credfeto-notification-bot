using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwitchLib.Api;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchStreamStatus : ITwitchStreamStatus
{
    private readonly TwitchAPI _api;
    private readonly ILogger<TwitchStreamStatus> _logger;
    private readonly LiveStreamMonitorService _lsm;
    private readonly IMediator _mediator;
    private readonly TwitchBotOptions _options;

    public TwitchStreamStatus(IOptions<TwitchBotOptions> options, IMediator mediator, ILogger<TwitchStreamStatus> logger)
    {
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

        List<string> channels = new[]
                                {
                                    this._options.Authentication.UserName
                                }.Concat(this._options.Channels.Select(c => c.ChannelName))
                                 .Select(c => c.ToLowerInvariant())
                                 .Distinct()
                                 .ToList();

        this._api = this._options.ConfigureTwitchApi();
        this._lsm = new(this._api);
        this._lsm.SetChannelsByName(channels);

        Observable.FromEventPattern<OnStreamOnlineArgs>(addHandler: h => this._lsm.OnStreamOnline += h, removeHandler: h => this._lsm.OnStreamOnline -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnStreamOnlineAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();

        Observable.FromEventPattern<OnStreamOfflineArgs>(addHandler: h => this._lsm.OnStreamOffline += h, removeHandler: h => this._lsm.OnStreamOffline -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Select(e => Observable.FromAsync(cancellationToken => this.OnStreamOfflineAsync(e: e, cancellationToken: cancellationToken)))
                  .Concat()
                  .Subscribe();
    }

    /// <inheritdoc />
    public Task UpdateAsync()
    {
        return this._lsm.UpdateLiveStreamersAsync();
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