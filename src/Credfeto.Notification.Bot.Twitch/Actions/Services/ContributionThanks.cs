using System;
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

    public Task ThankForPrimeReSubAsync(string channel, string user, in CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task ThankForPaidReSubAsync(string channel, string user, in CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task ThankForPaidSubAsync(string channel, string user, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task ThankForMultipleGiftSubsAsync(string channelName, string giftedBy, int count, in CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task ThankForGiftingSubAsync(string channelName, string giftedBy, in CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}