using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using MediatR;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchNewPrimeSubNotificationHandlerTests : TestBase
{
    private const string CHANNEL = nameof(CHANNEL);
    private const string SUBSCRIBER = nameof(SUBSCRIBER);
    private readonly IContributionThanks _contributionThanks;
    private readonly INotificationHandler<TwitchNewPrimeSub> _notificationHandler;

    public TwitchNewPrimeSubNotificationHandlerTests()
    {
        this._contributionThanks = GetSubstitute<IContributionThanks>();

        this._notificationHandler = new TwitchNewPrimeSubNotificationHandler(contributionThanks: this._contributionThanks, this.GetTypedLogger<TwitchNewPrimeSubNotificationHandler>());
    }

    [Fact]
    public async Task HandleAsync()
    {
        await this._notificationHandler.Handle(new(channel: CHANNEL, user: SUBSCRIBER), cancellationToken: CancellationToken.None);

        await this.ThankForNewPrimeSubAsync();
    }

    private Task ThankForNewPrimeSubAsync()
    {
        return this._contributionThanks.Received(1)
                   .ThankForNewPrimeSubAsync(channel: CHANNEL, user: SUBSCRIBER, Arg.Any<CancellationToken>());
    }
}