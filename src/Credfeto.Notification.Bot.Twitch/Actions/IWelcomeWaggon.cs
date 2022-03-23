using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch.Actions;

public interface IWelcomeWaggon
{
    Task IssueWelcomeAsync(string channel, string user, CancellationToken cancellationToken);
}