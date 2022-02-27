using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch.Actions;

public interface IRaidWelcome
{
    Task IssueRaidWelcomeAsync(string channel, string raider, CancellationToken cancellationToken);
}