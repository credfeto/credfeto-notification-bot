using System;
using System.Threading;
using System.Threading.Tasks;
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

public sealed class TwitchGiftSubMultipleNotificationHandlerTests : TestBase
{
    private static readonly Streamer Streamer = Streamer.FromString(nameof(Streamer));
    private static readonly Viewer GiftedBy = Viewer.FromString(nameof(GiftedBy));
    private readonly IContributionThanks _contributionThanks;
    private readonly INotificationHandler<TwitchGiftSubMultiple> _notificationHandler;

    public TwitchGiftSubMultipleNotificationHandlerTests()
    {
        this._contributionThanks = GetSubstitute<IContributionThanks>();

        this._notificationHandler =
            new TwitchGiftSubMultipleNotificationHandler(contributionThanks: this._contributionThanks, this.GetTypedLogger<TwitchGiftSubMultipleNotificationHandler>());
    }

    [Fact]
    public async Task HandleAsync()
    {
        await this._notificationHandler.Handle(new(streamer: Streamer, user: GiftedBy, count: 42), cancellationToken: CancellationToken.None);

        await this.ThankForMultipleGiftSubsAsync();
    }

    [Fact]
    public async Task HandleExceptionAsync()
    {
        this._contributionThanks.ThankForMultipleGiftSubsAsync(Arg.Any<Streamer>(), Arg.Any<Viewer>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Throws<TimeoutException>();

        await this._notificationHandler.Handle(new(streamer: Streamer, user: GiftedBy, count: 42), cancellationToken: CancellationToken.None);

        await this.ThankForMultipleGiftSubsAsync();
    }

    private Task ThankForMultipleGiftSubsAsync()
    {
        return this._contributionThanks.Received(1)
                   .ThankForMultipleGiftSubsAsync(streamer: Streamer, giftedBy: GiftedBy, count: 42, Arg.Any<CancellationToken>());
    }
}