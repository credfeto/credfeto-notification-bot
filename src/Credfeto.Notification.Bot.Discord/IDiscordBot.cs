using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Discord;

/// <summary>
///     Discord bot.
/// </summary>
public interface IDiscordBot
{
    /// <summary>
    ///     Publishes a message.
    /// </summary>
    /// <param name="message">The message to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PublishAsync(DiscordMessage message, CancellationToken cancellationToken);
}