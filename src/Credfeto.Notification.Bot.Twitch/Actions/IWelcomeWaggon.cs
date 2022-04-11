using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Actions;

public interface IWelcomeWaggon
{
    Task IssueWelcomeAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken);
}