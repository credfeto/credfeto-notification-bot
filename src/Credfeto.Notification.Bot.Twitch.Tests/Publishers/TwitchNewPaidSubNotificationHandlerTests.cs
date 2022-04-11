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

public sealed class TwitchNewPaidSubNotificationHandlerTests : TestBase
{
    private static readonly Channel Channel = Types.ChannelFromString(nameof(Channel));
    private static readonly User Subscriber = Types.UserFromString(nameof(Subscriber));
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
        await this._notificationHandler.Handle(new(channel: Channel, user: Subscriber), cancellationToken: CancellationToken.None);

        await this.ThankForNewPaidSubAsync();
    }

    private Task ThankForNewPaidSubAsync()
    {
        return this._contributionThanks.Received(1)
                   .ThankForNewPaidSubAsync(channel: Channel, user: Subscriber, Arg.Any<CancellationToken>());
    }
}