using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Models;

namespace Credfeto.Notification.Bot.Twitch;

public interface IUserInfoService
{
    Task<TwitchUser?> GetUserAsync(string userName);
}