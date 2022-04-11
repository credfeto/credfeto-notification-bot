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

public sealed class StreamLabsHeistStartingNotificationHandlerTests : TestBase
{
    private static readonly Streamer Streamer = Streamer.FromString(nameof(Streamer));
    private readonly IHeistJoiner _heistJoiner;
    private readonly INotificationHandler<StreamLabsHeistStarting> _notificationHandler;

    public StreamLabsHeistStartingNotificationHandlerTests()
    {
        this._heistJoiner = GetSubstitute<IHeistJoiner>();

        this._notificationHandler = new StreamLabsHeistStartingNotificationHandler(heistJoiner: this._heistJoiner, this.GetTypedLogger<StreamLabsHeistStartingNotificationHandler>());
    }

    [Fact]
    public async Task HandleAsync()
    {
        await this._notificationHandler.Handle(new(Streamer), cancellationToken: CancellationToken.None);

        await this.ReceivedJoinHeistAsync();
    }

    private Task ReceivedJoinHeistAsync()
    {
        return this._heistJoiner.Received(1)
                   .JoinHeistAsync(streamer: Streamer, Arg.Any<CancellationToken>());
    }
}