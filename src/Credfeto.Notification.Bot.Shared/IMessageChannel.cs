using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch.Resources;

/// <summary>
///     A Message channel.
/// </summary>
/// <typeparam name="T">Type of the message.</typeparam>
public interface IMessageChannel<T>
{
    /// <summary>
    ///     Publish a message to the channel.
    /// </summary>
    /// <param name="message">The message to publish.</param>
    /// <param name="cancellationToken">The cancellation token,</param>
    ValueTask PublishAsync(T message, CancellationToken cancellationToken);

    /// <summary>
    ///     Receives a message from the channel.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The message.</returns>
    ValueTask<T> ReceiveAsync(CancellationToken cancellationToken);

    IAsyncEnumerable<T> ReadAllAsync(in CancellationToken cancellationToken);
}