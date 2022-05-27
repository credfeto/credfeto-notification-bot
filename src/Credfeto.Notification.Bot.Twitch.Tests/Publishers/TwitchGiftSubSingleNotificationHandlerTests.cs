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

public sealed class TwitchGiftSubSingleNotificationHandlerTests : TestBase
{
    private static readonly Viewer GiftedBy = Viewer.FromString(nameof(GiftedBy));
    private readonly IContributionThanks _contributionThanks;
    private readonly INotificationHandler<TwitchGiftSubSingle> _notificationHandler;

    public TwitchGiftSubSingleNotificationHandlerTests()
    {
        this._contributionThanks = GetSubstitute<IContributionThanks>();

        this._notificationHandler =
            new TwitchGiftSubSingleNotificationHandler(contributionThanks: this._contributionThanks, this.GetTypedLogger<TwitchGiftSubSingleNotificationHandler>());
    }

    [Fact]
    public async Task HandleAsync()
    {
        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, user: GiftedBy), cancellationToken: CancellationToken.None);

        await this.ReceivedThankForGiftingSubAsync();
    }

    [Fact]
    public async Task HandleExceptionAsync()
    {
        this._contributionThanks.ThankForGiftingSubAsync(Arg.Any<Streamer>(), Arg.Any<Viewer>(), Arg.Any<CancellationToken>())
            .Throws<TimeoutException>();

        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, user: GiftedBy), cancellationToken: CancellationToken.None);

        await this.ReceivedThankForGiftingSubAsync();
    }

    private Task ReceivedThankForGiftingSubAsync()
    {
        return this._contributionThanks.Received(1)
                   .ThankForGiftingSubAsync(streamer: MockReferenceData.Streamer, giftedBy: GiftedBy, Arg.Any<CancellationToken>());
    }
}