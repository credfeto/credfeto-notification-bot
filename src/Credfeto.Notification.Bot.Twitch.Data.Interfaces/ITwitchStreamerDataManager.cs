using System;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch.Data.Interfaces;

public interface ITwitchStreamerDataManager
{
    Task AddStreamerAsync(string streamerName, DateTime startedStreaming);
}