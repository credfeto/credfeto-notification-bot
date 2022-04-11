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

public sealed class TwitchNewPrimeSubNotificationHandlerTests : TestBase
{
    private static readonly Streamer Streamer = Streamer.FromString(nameof(Streamer));
    private static readonly Viewer Subscriber = Viewer.FromString(nameof(Subscriber));

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
        await this._notificationHandler.Handle(new(streamer: Streamer, user: Subscriber), cancellationToken: CancellationToken.None);

        await this.ThankForNewPrimeSubAsync();
    }

    private Task ThankForNewPrimeSubAsync()
    {
        return this._contributionThanks.Received(1)
                   .ThankForNewPrimeSubAsync(streamer: Streamer, user: Subscriber, Arg.Any<CancellationToken>());
    }
}