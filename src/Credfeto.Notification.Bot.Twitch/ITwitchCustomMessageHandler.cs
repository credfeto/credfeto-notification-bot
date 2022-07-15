using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Models;

namespace Credfeto.Notification.Bot.Twitch;

public interface ITwitchCustomMessageHandler
{
    Task<bool> HandleMessageAsync(TwitchIncomingMessage message, CancellationToken cancellationToken);
}