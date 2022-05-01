using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Actions;

public interface IShoutoutJoiner
{
    Task<bool> IssueShoutoutAsync(Streamer streamer, TwitchUser visitingStreamer, bool isRegular, CancellationToken cancellationToken);
}