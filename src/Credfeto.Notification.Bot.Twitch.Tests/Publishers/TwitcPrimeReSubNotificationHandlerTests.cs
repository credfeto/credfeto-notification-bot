using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchPrimeReSubNotificationHandlerTests : TestBase
{
    private static readonly Viewer Subscriber = Viewer.FromString(nameof(Subscriber));
    private readonly IContributionThanks _contributionThanks;
    private readonly INotificationHandler<TwitchPrimeReSub> _notificationHandler;

    public TwitchPrimeReSubNotificationHandlerTests()
    {
        this._contributionThanks = GetSubstitute<IContributionThanks>();

        this._notificationHandler =
            new TwitchPrimeReSubNotificationHandler(contributionThanks: this._contributionThanks, this.GetTypedLogger<TwitchPrimeReSubNotificationHandler>());
    }

    [Fact]
    public async Task HandleAsync()
    {
        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, user: Subscriber), cancellationToken: CancellationToken.None);

        await this.ReceivedThankForPrimeReSubAsync();
    }

    [Fact]
    public async Task HandleExceptionAsync()
    {
        this._contributionThanks.ThankForPrimeReSubAsync(Arg.Any<Streamer>(), Arg.Any<Viewer>(), Arg.Any<CancellationToken>())
            .Throws<TimeoutException>();

        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, user: Subscriber), cancellationToken: CancellationToken.None);

        await this.ReceivedThankForPrimeReSubAsync();
    }

    private Task ReceivedThankForPrimeReSubAsync()
    {
        return this._contributionThanks.Received(1)
                   .ThankForPrimeReSubAsync(streamer: MockReferenceData.Streamer, user: Subscriber, Arg.Any<CancellationToken>());
    }
}