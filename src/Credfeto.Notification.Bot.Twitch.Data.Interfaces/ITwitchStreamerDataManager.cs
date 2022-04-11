using System;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Data.Interfaces;

public interface ITwitchStreamerDataManager
{
    Task AddStreamerAsync(Channel streamerName, string streamerId, DateTime startedStreaming);

    Task<TwitchUser?> GetByUserNameAsync(Channel userName);

    Task<TwitchUser?> GetByUserNameAsync(User userName);
}