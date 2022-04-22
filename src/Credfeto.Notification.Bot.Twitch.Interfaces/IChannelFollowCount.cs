using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Interfaces;

public interface IChannelFollowCount
{
    Task<int> GetCurrentFollowerCountAsync(Streamer streamer, CancellationToken cancellationToken);
}