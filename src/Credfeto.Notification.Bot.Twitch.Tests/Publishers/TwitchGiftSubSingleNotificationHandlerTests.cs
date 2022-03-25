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

public sealed class TwitchGiftSubSingleNotificationHandlerTests : TestBase
{
    private const string CHANNEL = nameof(CHANNEL);
    private const string GIFTED_BY = nameof(GIFTED_BY);
    private readonly IContributionThanks _contributionThanks;
    private readonly INotificationHandler<TwitchGiftSubSingle> _notificationHandler;

    public TwitchGiftSubSingleNotificationHandlerTests()
    {
        this._contributionThanks = GetSubstitute<IContributionThanks>();

        this._notificationHandler = new TwitchGiftSubSingleNotificationHandler(contributionThanks: this._contributionThanks, this.GetTypedLogger<TwitchGiftSubSingleNotificationHandler>());
    }

    [Fact]
    public async Task HandleAsync()
    {
        await this._notificationHandler.Handle(new(channel: CHANNEL, user: GIFTED_BY), cancellationToken: CancellationToken.None);

        await this.ThankForGiftingSubAsync();
    }

    private Task ThankForGiftingSubAsync()
    {
        return this._contributionThanks.Received(1)
                   .ThankForGiftingSubAsync(channel: CHANNEL, giftedBy: GIFTED_BY, Arg.Any<CancellationToken>());
    }
}