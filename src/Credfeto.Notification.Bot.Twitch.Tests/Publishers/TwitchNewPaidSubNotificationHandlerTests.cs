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

public sealed class TwitchNewPaidSubNotificationHandlerTests : TestBase
{
    private const string CHANNEL = nameof(CHANNEL);
    private const string SUBSCRIBER = nameof(SUBSCRIBER);
    private readonly IContributionThanks _contributionThanks;
    private readonly INotificationHandler<TwitchNewPaidSub> _notificationHandler;

    public TwitchNewPaidSubNotificationHandlerTests()
    {
        this._contributionThanks = GetSubstitute<IContributionThanks>();

        this._notificationHandler = new TwitchNewPaidSubNotificationHandler(contributionThanks: this._contributionThanks, this.GetTypedLogger<TwitchNewPaidSubNotificationHandler>());
    }

    [Fact]
    public async Task HandleAsync()
    {
        await this._notificationHandler.Handle(new(channel: CHANNEL, user: SUBSCRIBER), cancellationToken: CancellationToken.None);

        await this.ThankForNewPaidSubAsync();
    }

    private Task ThankForNewPaidSubAsync()
    {
        return this._contributionThanks.Received(1)
                   .ThankForNewPaidSubAsync(channel: CHANNEL, user: SUBSCRIBER, Arg.Any<CancellationToken>());
    }
}