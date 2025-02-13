using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Shared;

public interface IMessageChannel<T>
{
    ValueTask PublishAsync(T message, CancellationToken cancellationToken);

    ValueTask<T> ReceiveAsync(CancellationToken cancellationToken);

    IAsyncEnumerable<T> ReadAllAsync(in CancellationToken cancellationToken);
}
