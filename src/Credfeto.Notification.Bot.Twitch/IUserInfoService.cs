using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;

namespace Credfeto.Notification.Bot.Twitch;

public interface IUserInfoService
{
    Task<TwitchUser?> GetUserAsync(string userName);
}