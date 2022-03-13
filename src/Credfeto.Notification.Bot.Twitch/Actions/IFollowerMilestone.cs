using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch.Actions;

public interface IFollowerMilestone
{
    Task IssueMilestoneUpdateAsync(string channel, int followers, CancellationToken cancellationToken);
}