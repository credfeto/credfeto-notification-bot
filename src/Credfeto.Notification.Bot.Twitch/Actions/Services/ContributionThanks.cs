using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Models;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class ContributionThanks : MessageSenderBase, IContributionThanks
{
    public ContributionThanks(IMessageChannel<TwitchChatMessage> twitchChatMessageChannel)
        : base(twitchChatMessageChannel)
    {
    }

    public Task ThankForBitsAsync(string channel, string user, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task ThankForPrimeSubAsync(string channel, string user, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task ThankForPaidSubAsync(string channel, string user, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}