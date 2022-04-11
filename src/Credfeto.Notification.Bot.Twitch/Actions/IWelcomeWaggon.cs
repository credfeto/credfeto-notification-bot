using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Actions;

public interface IWelcomeWaggon
{
    Task IssueWelcomeAsync(Channel channel, User user, CancellationToken cancellationToken);
}