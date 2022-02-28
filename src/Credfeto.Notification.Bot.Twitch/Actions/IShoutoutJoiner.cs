using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch.Actions;

public interface IShoutoutJoiner
{
    Task<bool> IssueShoutoutAsync(string channel, string visitingStreamer, CancellationToken cancellationToken);
}