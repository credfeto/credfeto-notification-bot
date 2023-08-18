using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Data.Interfaces;

public interface ITwitchViewerDataManager
{
    ValueTask AddViewerAsync(Viewer viewerName, string viewerId, DateTimeOffset dateCreated, CancellationToken cancellationToken);

    ValueTask<TwitchUser?> GetByUserNameAsync(Viewer userName, CancellationToken cancellationToken);
}