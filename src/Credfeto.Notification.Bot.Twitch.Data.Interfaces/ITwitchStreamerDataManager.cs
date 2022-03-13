using System;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch.Data.Interfaces;

public interface ITwitchStreamerDataManager
{
    Task AddStreamerAsync(string streamerName, string streamerId, DateTime startedStreaming);

    Task<TwitchUser?> GetByUserNameAsync(string userName);
}