using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Actions;

public interface IHoster
{
    Task StreamOnlineAsync(Streamer streamer, CancellationToken cancellationToken);

    Task StreamOfflineAsync(Streamer streamer, CancellationToken cancellationToken);
}