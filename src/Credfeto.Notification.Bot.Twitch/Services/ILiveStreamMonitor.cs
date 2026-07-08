using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;

namespace Credfeto.Notification.Bot.Twitch.Services;

public interface ILiveStreamMonitor
{
    event EventHandler<OnStreamOnlineArgs>? OnStreamOnline;
    event EventHandler<OnStreamOfflineArgs>? OnStreamOffline;
    void SetChannelsByName(List<string> channelsToMonitor);
    Task UpdateLiveStreamersAsync();
}
