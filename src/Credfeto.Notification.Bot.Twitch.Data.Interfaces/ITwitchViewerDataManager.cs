using System;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Data.Interfaces;

public interface ITwitchViewerDataManager
{
    Task AddViewerAsync(Viewer viewerName, string viewerId, DateTimeOffset dateCreated);

    Task<TwitchUser?> GetByUserNameAsync(Viewer userName);
}