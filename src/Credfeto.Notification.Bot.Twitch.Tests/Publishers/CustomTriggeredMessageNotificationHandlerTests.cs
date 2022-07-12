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

public sealed class CustomTriggeredMessageNotificationHandlerTests : TestBase
{
    private const string MESSAGE = "!hello";
    private readonly ICustomTriggeredMessageSender _customTriggeredMessageSender;
    private readonly INotificationHandler<CustomTriggeredMessage> _notificationHandler;

    public CustomTriggeredMessageNotificationHandlerTests()
    {
        this._customTriggeredMessageSender = GetSubstitute<ICustomTriggeredMessageSender>();

        this._notificationHandler =
            new CustomTriggeredMessageNotificationHandler(customTriggeredMessageSender: this._customTriggeredMessageSender, this.GetTypedLogger<CustomTriggeredMessageNotificationHandler>());
    }

    [Fact]
    public async Task HandleAsync()
    {
        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, message: MESSAGE), cancellationToken: CancellationToken.None);

        await this.ReceivedJoinHeistAsync();
    }

    [Fact]
    public async Task HandleExceptionAsync()
    {
        this._customTriggeredMessageSender.SendAsync(Arg.Any<Streamer>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Throws<TimeoutException>();

        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer, message: MESSAGE), cancellationToken: CancellationToken.None);

        await this.ReceivedJoinHeistAsync();
    }

    private Task ReceivedJoinHeistAsync()
    {
        return this._customTriggeredMessageSender.Received(1)
                   .SendAsync(streamer: MockReferenceData.Streamer, message: MESSAGE, Arg.Any<CancellationToken>());
    }
}