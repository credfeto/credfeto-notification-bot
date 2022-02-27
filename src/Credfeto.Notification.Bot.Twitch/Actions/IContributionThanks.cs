using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch.Actions;

public interface IContributionThanks
{
    Task ThankForBitsAsync(string channel, string user, CancellationToken cancellationToken);

    Task ThankForPrimeSubAsync(string channel, string user, CancellationToken cancellationToken);

    Task ThankForPrimeReSubAsync(string channel, string user, in CancellationToken cancellationToken);

    Task ThankForPaidReSubAsync(string channel, string user, in CancellationToken cancellationToken);

    Task ThankForPaidSubAsync(string channel, string user, CancellationToken cancellationToken);

    Task ThankForMultipleGiftSubsAsync(string channelName, string giftedBy, int count, in CancellationToken cancellationToken);

    Task ThankForGiftingSubAsync(string channelName, string giftedBy, in CancellationToken cancellationToken);
}