using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using MediatR;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchBitsGiftNotificationHandlerTests : TestBase
{
    private static readonly Streamer Streamer = Streamer.FromString(nameof(Streamer));
    private static readonly Viewer GiftedBy = Viewer.FromString(nameof(GiftedBy));
    private readonly IContributionThanks _contributionThanks;
    private readonly INotificationHandler<TwitchBitsGift> _notificationHandler;

    public TwitchBitsGiftNotificationHandlerTests()
    {
        this._contributionThanks = GetSubstitute<IContributionThanks>();

        this._notificationHandler = new TwitchBitsGiftNotificationHandler(contributionThanks: this._contributionThanks, this.GetTypedLogger<TwitchBitsGiftNotificationHandler>());
    }

    [Fact]
    public async Task HandleAsync()
    {
        await this._notificationHandler.Handle(new(streamer: Streamer, user: GiftedBy, bits: 404), cancellationToken: CancellationToken.None);

        await this.ReceivedThankForBitsAsync();
    }

    private Task ReceivedThankForBitsAsync()
    {
        return this._contributionThanks.Received(1)
                   .ThankForBitsAsync(streamer: Streamer, user: GiftedBy, Arg.Any<CancellationToken>());
    }
}