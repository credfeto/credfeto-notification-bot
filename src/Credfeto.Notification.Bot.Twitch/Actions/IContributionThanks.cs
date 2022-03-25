using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch.Actions;

public interface IContributionThanks
{
    Task ThankForBitsAsync(string channel, string user, CancellationToken cancellationToken);

    Task ThankForNewPrimeSubAsync(string channel, string user, CancellationToken cancellationToken);

    Task ThankForPrimeReSubAsync(string channel, string user, CancellationToken cancellationToken);

    Task ThankForPaidReSubAsync(string channel, string user, CancellationToken cancellationToken);

    Task ThankForNewPaidSubAsync(string channel, string user, CancellationToken cancellationToken);

    Task ThankForMultipleGiftSubsAsync(string channel, string giftedBy, int count, CancellationToken cancellationToken);

    Task ThankForGiftingSubAsync(string channel, string giftedBy, CancellationToken cancellationToken);

    Task ThankForFollowAsync(string channel, string user, CancellationToken cancellationToken);
}