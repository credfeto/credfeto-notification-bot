using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Interfaces;

public interface ITwitchChannelState
{
    Streamer Streamer { get; }

    bool IsOnline { get; }

    ITwitchStreamSettings Settings { get; }

    Task SaveStreamSettingsAsync();

    Task OnlineAsync(string gameName, DateTime startDate);

    void Offline();

    void ClearChat();

    Task RaidedAsync(Viewer raider, int viewerCount, CancellationToken cancellationToken);

    Task ChatMessageAsync(Viewer user, string message, CancellationToken cancellationToken);

    Task BitsGiftedAsync(Viewer user, int bits, CancellationToken cancellationToken);

    Task GiftedMultipleAsync(Viewer giftedBy, int count, string months, in CancellationToken cancellationToken);

    Task GiftedSubAsync(Viewer giftedBy, string months, in CancellationToken cancellationToken);

    Task ContinuedSubAsync(Viewer user, in CancellationToken cancellationToken);

    Task PrimeToPaidAsync(Viewer user, in CancellationToken cancellationToken);

    Task NewSubscriberPaidAsync(Viewer user, in CancellationToken cancellationToken);

    Task NewSubscriberPrimeAsync(Viewer user, in CancellationToken cancellationToken);

    Task ResubscribePaidAsync(Viewer user, int months, in CancellationToken cancellationToken);

    Task ResubscribePrimeAsync(Viewer user, int months, in CancellationToken cancellationToken);

    Task NewFollowerAsync(Viewer user, CancellationToken cancellationToken);
}