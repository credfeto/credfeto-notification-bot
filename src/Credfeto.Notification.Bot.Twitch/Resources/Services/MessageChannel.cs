using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch.Resources.Services;

/// <summary>
///     A Message channel.
/// </summary>
/// <typeparam name="T">Type of the message.</typeparam>
public sealed class MessageChannel<T> : IMessageChannel<T>
{
    private readonly Channel<T> _channel;

    /// <summary>
    ///     Constructor.
    /// </summary>
    public MessageChannel()
    {
        this._channel = Channel.CreateUnbounded<T>();
    }

    /// <inheritdoc />
    public ValueTask PublishAsync(T message, CancellationToken cancellationToken)
    {
        return this._channel.Writer.WriteAsync(item: message, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<T> ReceiveAsync(CancellationToken cancellationToken)
    {
        return this._channel.Reader.ReadAsync(cancellationToken);
    }

    public IAsyncEnumerable<T> ReadAllAsync(in CancellationToken cancellationToken)
    {
        return this._channel.Reader.ReadAllAsync(cancellationToken);
    }
}