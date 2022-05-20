using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Discord.Models;

namespace Credfeto.Notification.Bot.Discord;

public interface IDiscordBot
{
    Task PublishAsync(DiscordMessage message, CancellationToken cancellationToken);
}