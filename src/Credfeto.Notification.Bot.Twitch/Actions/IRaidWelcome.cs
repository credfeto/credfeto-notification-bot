using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Actions;

public interface IRaidWelcome
{
    Task IssueRaidWelcomeAsync(Channel channel, User raider, CancellationToken cancellationToken);
}