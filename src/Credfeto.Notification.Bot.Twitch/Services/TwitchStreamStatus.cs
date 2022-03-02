using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.Models;
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
    private readonly TwitchBotOptions _options;
    private readonly ITwitchChannelManager _twitchChannelManager;

    public TwitchStreamStatus(IOptions<TwitchBotOptions> options, ITwitchChannelManager twitchChannelManager, ILogger<TwitchStreamStatus> logger)
    {
        this._twitchChannelManager = twitchChannelManager ?? throw new ArgumentNullException(nameof(twitchChannelManager));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

        List<string> channels = new[]
                                {
                                    this._options.Authentication.UserName
                                }.Concat(this._options.Channels)
                                 .Select(c => c.ToLowerInvariant())
                                 .Distinct()
                                 .ToList();

        this._api = OptionsExtensions.ConfigureTwitchApi(this._options);
        this._lsm = new(this._api);
        this._lsm.SetChannelsByName(channels);

        Observable.FromEventPattern<OnStreamOnlineArgs>(addHandler: h => this._lsm.OnStreamOnline += h, removeHandler: h => this._lsm.OnStreamOnline -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.OnStreamOnline);

        Observable.FromEventPattern<OnStreamOfflineArgs>(addHandler: h => this._lsm.OnStreamOffline += h, removeHandler: h => this._lsm.OnStreamOffline -= h)
                  .Select(messageEvent => messageEvent.EventArgs)
                  .Subscribe(this.OnStreamOffline);
    }

    /// <inheritdoc />
    public Task UpdateAsync()
    {
        this._logger.LogDebug("Tick...");

        return this._lsm.UpdateLiveStreamersAsync();

        //return Task.CompletedTask;
    }

    private void OnStreamOnline(OnStreamOnlineArgs e)
    {
        this._logger.LogWarning($"{e.Channel}: Started streaming \"{e.Stream.Title}\" ({e.Stream.GameName}) at {e.Stream.StartedAt}");

        TwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        state.Online(gameName: e.Stream.GameName, startDate: e.Stream.StartedAt);
    }

    private void OnStreamOffline(OnStreamOfflineArgs e)
    {
        this._logger.LogWarning($"{e.Channel}: Stopped streaming {e.Stream.Title} ({e.Stream.GameName})");

        TwitchChannelState state = this._twitchChannelManager.GetChannel(e.Channel);

        state.Offline();
    }
}