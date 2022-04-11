using System;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Data.Interfaces;

public interface ITwitchStreamerDataManager
{
    Task AddStreamerAsync(Streamer streamerName, string streamerId, DateTime startedStreaming);

    Task<TwitchUser?> GetByUserNameAsync(Streamer userName);

    Task<TwitchUser?> GetByUserNameAsync(Viewer userName);
}