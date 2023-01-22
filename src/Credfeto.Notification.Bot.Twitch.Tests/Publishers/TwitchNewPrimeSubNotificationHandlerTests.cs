using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using Mediator;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchNewPrimeSubNotificationHandlerTests : TestBase
{
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
        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer), cancellationToken: CancellationToken.None);

        await this.ReceivedThankForNewPrimeSubAsync();
    }

    [Fact]
    public async Task HandleExceptionAsync()
    {
        this._contributionThanks.ThankForNewPrimeSubAsync(Arg.Any<Streamer>(), Arg.Any<Viewer>(), Arg.Any<CancellationToken>())
            .ThrowsAsync<TimeoutException>();

        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer), cancellationToken: CancellationToken.None);

        await this.ReceivedThankForNewPrimeSubAsync();
    }

    private Task ReceivedThankForNewPrimeSubAsync()
    {
        return this._contributionThanks.Received(1)
                   .ThankForNewPrimeSubAsync(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, Arg.Any<CancellationToken>());
    }
}