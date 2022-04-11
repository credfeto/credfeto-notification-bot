using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch;

public interface IUserInfoService
{
    Task<TwitchUser?> GetUserAsync(User userName);

    Task<TwitchUser?> GetUserAsync(Channel userName);
}