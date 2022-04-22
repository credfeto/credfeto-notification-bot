using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Interfaces;

public interface IUserInfoService
{
    Task<TwitchUser?> GetUserAsync(Viewer userName);

    Task<TwitchUser?> GetUserAsync(Streamer userName);
}