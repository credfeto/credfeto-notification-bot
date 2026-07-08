using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Microsoft.Extensions.Options;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class LiveStreamMonitorAdapter : ILiveStreamMonitor
{
    private readonly LiveStreamMonitorService _service;

    public LiveStreamMonitorAdapter(IOptions<TwitchBotOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        this._service = new(options.Value.ConfigureTwitchApi());
    }

    public event EventHandler<OnStreamOnlineArgs>? OnStreamOnline
    {
        add => this._service.OnStreamOnline += value;
        remove => this._service.OnStreamOnline -= value;
    }

    public event EventHandler<OnStreamOfflineArgs>? OnStreamOffline
    {
        add => this._service.OnStreamOffline += value;
        remove => this._service.OnStreamOffline -= value;
    }

    public void SetChannelsByName(List<string> channelsToMonitor)
    {
        this._service.SetChannelsByName(channelsToMonitor);
    }

    public Task UpdateLiveStreamersAsync()
    {
        return this._service.UpdateLiveStreamersAsync();
    }
}
