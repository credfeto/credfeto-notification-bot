using System.Threading;
using System.Threading.Tasks;

namespace Credfeto.Notification.Bot.Twitch.Actions;

public interface IContributionThanks
{
    Task ThankForBitsAsync(string channel, string user, CancellationToken cancellationToken);

    Task ThankForPrimeSubAsync(string channel, string user, CancellationToken cancellationToken);

    Task ThankForPaidSubAsync(string channel, string user, CancellationToken cancellationToken);
}