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

    Task SaveStreamSettingsAsync(in CancellationToken cancellationToken);

    Task OnlineAsync(string gameName, DateTimeOffset startDate, CancellationToken cancellationToken);

    void Offline();

    void ClearChat();

    ValueTask RaidedAsync(Viewer raider, int viewerCount, CancellationToken cancellationToken);

    ValueTask ChatMessageAsync(Viewer user, string message, CancellationToken cancellationToken);

    ValueTask BitsGiftedAsync(Viewer user, int bits, CancellationToken cancellationToken);

    ValueTask GiftedMultipleAsync(Viewer giftedBy, int count, string months, in CancellationToken cancellationToken);

    ValueTask GiftedSubAsync(Viewer giftedBy, string months, in CancellationToken cancellationToken);

    ValueTask ContinuedSubAsync(Viewer user, in CancellationToken cancellationToken);

    ValueTask PrimeToPaidAsync(Viewer user, in CancellationToken cancellationToken);

    ValueTask NewSubscriberPaidAsync(Viewer user, in CancellationToken cancellationToken);

    ValueTask NewSubscriberPrimeAsync(Viewer user, in CancellationToken cancellationToken);

    ValueTask ResubscribePaidAsync(Viewer user, int months, in CancellationToken cancellationToken);

    ValueTask ResubscribePrimeAsync(Viewer user, int months, in CancellationToken cancellationToken);

    ValueTask NewFollowerAsync(Viewer user, CancellationToken cancellationToken);
}