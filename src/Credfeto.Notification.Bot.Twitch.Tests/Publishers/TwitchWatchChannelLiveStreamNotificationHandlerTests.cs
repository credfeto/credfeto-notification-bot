using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using Mediator;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchWatchChannelLiveStreamNotificationHandlerTests : TestBase
{
    private readonly INotificationHandler<TwitchWatchChannel> _notificationHandler;
    private readonly ITwitchStreamStatus _twitchStreamStatus;

    public TwitchWatchChannelLiveStreamNotificationHandlerTests()
    {
        this._twitchStreamStatus = GetSubstitute<ITwitchStreamStatus>();

        this._notificationHandler = new TwitchWatchChannelLiveStreamNotificationHandler(twitchStreamStatus: this._twitchStreamStatus,
                                                                                        this.GetTypedLogger<TwitchWatchChannelLiveStreamNotificationHandler>());
    }

    [Fact]
    public async Task ShouldEnableStreamStatusAsync()
    {
        Viewer viewer = MockReferenceData.Viewer;
        TwitchWatchChannel notification = new(new(Id: 42, UserName: viewer, IsStreamer: false, DateCreated: DateTime.MinValue));
        await this._notificationHandler.Handle(notification: notification, cancellationToken: CancellationToken.None);

        await this._twitchStreamStatus.Received(1)
                  .EnableAsync(viewer.ToStreamer());
    }
}