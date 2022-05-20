using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Shared;

public interface IRunOnStartup
{
    Task StartAsync(CancellationToken cancellationToken);
}