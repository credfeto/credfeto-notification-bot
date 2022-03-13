using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch;

public interface IChannelFollowCount
{
    Task<int> GetCurrentFollowerCountAsync(string username, CancellationToken cancellationToken);
}