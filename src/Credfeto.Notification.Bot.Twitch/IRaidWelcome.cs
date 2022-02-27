using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch;

public interface IRaidWelcome
{
    Task IssueRaidWelcomeAsync(string channel, string raider, CancellationToken cancellationToken);
}

public interface IHeistJoiner
{
    Task JoinHeistAsync(string channel, CancellationToken cancellationToken);
}