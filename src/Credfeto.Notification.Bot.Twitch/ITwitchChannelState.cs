using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch;

public interface ITwitchChannelState
{
    Task OnlineAsync(string gameName, in DateTime startDate);

    void Offline();

    void ClearChat();

    Task RaidedAsync(User raider, int viewerCount, CancellationToken cancellationToken);

    Task ChatMessageAsync(User user, string message, int bits, CancellationToken cancellationToken);

    Task GiftedMultipleAsync(User giftedBy, int count, string months, in CancellationToken cancellationToken);

    Task GiftedSubAsync(User giftedBy, string months, in CancellationToken cancellationToken);

    Task ContinuedSubAsync(User user, in CancellationToken cancellationToken);

    Task PrimeToPaidAsync(User user, in CancellationToken cancellationToken);

    Task NewSubscriberPaidAsync(User user, in CancellationToken cancellationToken);

    Task NewSubscriberPrimeAsync(User user, in CancellationToken cancellationToken);

    Task ResubscribePaidAsync(User user, int months, in CancellationToken cancellationToken);

    Task ResubscribePrimeAsync(User user, int months, in CancellationToken cancellationToken);

    Task NewFollowerAsync(User user, CancellationToken cancellationToken);
}