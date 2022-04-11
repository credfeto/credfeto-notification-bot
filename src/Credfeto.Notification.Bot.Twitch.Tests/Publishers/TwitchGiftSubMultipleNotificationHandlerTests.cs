using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using MediatR;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchGiftSubMultipleNotificationHandlerTests : TestBase
{
    private static readonly Channel Channel = Types.ChannelFromString(nameof(Channel));
    private static readonly User GiftedBy = Types.UserFromString(nameof(GiftedBy));
    private readonly IContributionThanks _contributionThanks;
    private readonly INotificationHandler<TwitchGiftSubMultiple> _notificationHandler;

    public TwitchGiftSubMultipleNotificationHandlerTests()
    {
        this._contributionThanks = GetSubstitute<IContributionThanks>();

        this._notificationHandler = new TwitchGiftSubMultipleNotificationHandler(contributionThanks: this._contributionThanks, this.GetTypedLogger<TwitchGiftSubMultipleNotificationHandler>());
    }

    [Fact]
    public async Task HandleAsync()
    {
        await this._notificationHandler.Handle(new(channel: Channel, user: GiftedBy, count: 42), cancellationToken: CancellationToken.None);

        await this.ThankForMultipleGiftSubsAsync();
    }

    private Task ThankForMultipleGiftSubsAsync()
    {
        return this._contributionThanks.Received(1)
                   .ThankForMultipleGiftSubsAsync(channel: Channel, giftedBy: GiftedBy, count: 42, Arg.Any<CancellationToken>());
    }
}