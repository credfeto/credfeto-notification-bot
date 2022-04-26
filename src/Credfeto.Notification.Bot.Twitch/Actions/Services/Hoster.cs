using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class Hoster : IHoster
{
    public Task StreamOnlineAsync(Streamer streamer, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StreamOfflineAsync(Streamer streamer, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}