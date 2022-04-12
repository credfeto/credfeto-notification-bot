using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Shared;

/// <summary>
///     Executes a task on startup
/// </summary>
public interface IRunOnStartup
{
    /// <summary>
    ///     Executes task.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task StartAsync(CancellationToken cancellationToken);
}