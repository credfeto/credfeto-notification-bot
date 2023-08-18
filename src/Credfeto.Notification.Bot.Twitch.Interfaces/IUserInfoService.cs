using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Interfaces;

public interface IUserInfoService
{
    Task<TwitchUser?> GetUserAsync(Viewer userName, CancellationToken cancellationToken);

    Task<TwitchUser?> GetUserAsync(in Streamer userName, in CancellationToken cancellationToken);
}