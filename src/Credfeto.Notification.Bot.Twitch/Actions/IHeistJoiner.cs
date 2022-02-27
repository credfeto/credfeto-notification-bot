using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch.Actions;

public interface IHeistJoiner
{
    Task JoinHeistAsync(string channel, CancellationToken cancellationToken);
}