using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;

namespace Credfeto.Notification.Bot.Twitch.Actions;

public interface IContributionThanks
{
    Task ThankForBitsAsync(Channel channel, User user, CancellationToken cancellationToken);

    Task ThankForNewPrimeSubAsync(Channel channel, User user, CancellationToken cancellationToken);

    Task ThankForPrimeReSubAsync(Channel channel, User user, CancellationToken cancellationToken);

    Task ThankForPaidReSubAsync(Channel channel, User user, CancellationToken cancellationToken);

    Task ThankForNewPaidSubAsync(Channel channel, User user, CancellationToken cancellationToken);

    Task ThankForMultipleGiftSubsAsync(Channel channel, User giftedBy, int count, CancellationToken cancellationToken);

    Task ThankForGiftingSubAsync(Channel channel, User giftedBy, CancellationToken cancellationToken);

    Task ThankForFollowAsync(Channel channel, User user, CancellationToken cancellationToken);
}