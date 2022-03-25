using System;
using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch;

public interface ITwitchChannelState
{
    Task OnlineAsync(string gameName, in DateTime startDate);

    void Offline();

    void ClearChat();

    Task RaidedAsync(string raider, int viewerCount, CancellationToken cancellationToken);

    Task ChatMessageAsync(string user, string message, int bits, CancellationToken cancellationToken);

    Task GiftedMultipleAsync(string giftedBy, int count, string months, in CancellationToken cancellationToken);

    Task GiftedSubAsync(string giftedBy, string months, in CancellationToken cancellationToken);

    Task ContinuedSubAsync(string user, in CancellationToken cancellationToken);

    Task PrimeToPaidAsync(string user, in CancellationToken cancellationToken);

    Task NewSubscriberPaidAsync(string user, in CancellationToken cancellationToken);

    Task NewSubscriberPrimeAsync(string user, in CancellationToken cancellationToken);

    Task ResubscribePaidAsync(string user, int months, in CancellationToken cancellationToken);

    Task ResubscribePrimeAsync(string user, int months, in CancellationToken cancellationToken);

    Task NewFollowerAsync(string user, in CancellationToken cancellationToken);
}