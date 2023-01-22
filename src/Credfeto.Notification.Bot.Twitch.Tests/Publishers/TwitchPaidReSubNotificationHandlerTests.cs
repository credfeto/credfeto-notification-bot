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

public sealed class TwitchPaidReSubNotificationHandlerTests : TestBase
{
    private readonly IContributionThanks _contributionThanks;
    private readonly INotificationHandler<TwitchPaidReSub> _notificationHandler;

    public TwitchPaidReSubNotificationHandlerTests()
    {
        this._contributionThanks = GetSubstitute<IContributionThanks>();

        this._notificationHandler = new TwitchPaidReSubNotificationHandler(contributionThanks: this._contributionThanks, this.GetTypedLogger<TwitchPaidReSubNotificationHandler>());
    }

    [Fact]
    public async Task HandleAsync()
    {
        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer), cancellationToken: CancellationToken.None);

        await this.ReceivedThankForPaidReSubAsync();
    }

    [Fact]
    public async Task HandleExceptionAsync()
    {
        this._contributionThanks.ThankForPaidReSubAsync(Arg.Any<Streamer>(), Arg.Any<Viewer>(), Arg.Any<CancellationToken>())
            .ThrowsAsync<TimeoutException>();

        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer), cancellationToken: CancellationToken.None);

        await this.ReceivedThankForPaidReSubAsync();
    }

    private Task ReceivedThankForPaidReSubAsync()
    {
        return this._contributionThanks.Received(1)
                   .ThankForPaidReSubAsync(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, Arg.Any<CancellationToken>());
    }
}