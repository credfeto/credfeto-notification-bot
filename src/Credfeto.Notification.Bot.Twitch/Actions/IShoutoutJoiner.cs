using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;

namespace Credfeto.Notification.Bot.Twitch.Actions;

public interface IShoutoutJoiner
{
    Task<bool> IssueShoutoutAsync(string channel, TwitchUser visitingStreamer, CancellationToken cancellationToken);
}