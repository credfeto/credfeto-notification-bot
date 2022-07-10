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

public sealed class MarblesStartingNotificationHandlerTests : TestBase
{
    private readonly IMarblesJoiner _marblesJoiner;
    private readonly INotificationHandler<MarblesStarting> _notificationHandler;

    public MarblesStartingNotificationHandlerTests()
    {
        this._marblesJoiner = GetSubstitute<IMarblesJoiner>();

        this._notificationHandler = new MarblesStartingNotificationHandler(marblesJoiner: this._marblesJoiner, this.GetTypedLogger<MarblesStartingNotificationHandler>());
    }

    [Fact]
    public async Task HandleAsync()
    {
        await this._notificationHandler.Handle(new(MockReferenceData.Streamer), cancellationToken: CancellationToken.None);

        await this.ReceivedJoinHeistAsync();
    }

    [Fact]
    public async Task HandleExceptionAsync()
    {
        this._marblesJoiner.JoinMarblesAsync(Arg.Any<Streamer>(), Arg.Any<CancellationToken>())
            .Throws<TimeoutException>();

        await this._notificationHandler.Handle(new(MockReferenceData.Streamer), cancellationToken: CancellationToken.None);

        await this.ReceivedJoinHeistAsync();
    }

    private Task ReceivedJoinHeistAsync()
    {
        return this._marblesJoiner.Received(1)
                   .JoinMarblesAsync(streamer: MockReferenceData.Streamer, Arg.Any<CancellationToken>());
    }
}