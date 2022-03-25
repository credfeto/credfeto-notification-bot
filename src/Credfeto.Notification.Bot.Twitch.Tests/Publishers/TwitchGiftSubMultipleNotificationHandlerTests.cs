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

public sealed class TwitchGiftSubMultipleNotificationHandlerTests : TestBase
{
    private const string CHANNEL = nameof(CHANNEL);
    private const string GIFTED_BY = nameof(GIFTED_BY);
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
        await this._notificationHandler.Handle(new(channel: CHANNEL, user: GIFTED_BY, count: 42), cancellationToken: CancellationToken.None);

        await this.ThankForMultipleGiftSubsAsync();
    }

    private Task ThankForMultipleGiftSubsAsync()
    {
        return this._contributionThanks.Received(1)
                   .ThankForMultipleGiftSubsAsync(channel: CHANNEL, giftedBy: GIFTED_BY, count: 42, Arg.Any<CancellationToken>());
    }
}