using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Actions;

public interface IContributionThanks
{
    Task ThankForBitsAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken);

    Task ThankForNewPrimeSubAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken);

    Task ThankForPrimeReSubAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken);

    Task ThankForPaidReSubAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken);

    Task ThankForNewPaidSubAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken);

    Task ThankForMultipleGiftSubsAsync(Streamer streamer, Viewer giftedBy, int count, CancellationToken cancellationToken);

    Task ThankForGiftingSubAsync(Streamer streamer, Viewer giftedBy, CancellationToken cancellationToken);

    Task ThankForFollowAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken);
}