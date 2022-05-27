using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using MediatR;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchWatchChannelLiveStreamNotificationHandlerTests : TestBase
{
    private static readonly Viewer Viewer = Viewer.FromString(nameof(Streamer));

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
        TwitchWatchChannel notification = new(new(id: "42", userName: Viewer, isStreamer: false, dateCreated: DateTime.MinValue));
        await this._notificationHandler.Handle(notification: notification, cancellationToken: CancellationToken.None);

        await this._twitchStreamStatus.Received(1)
                  .EnableAsync(MockReferenceData.Streamer);
    }
}