using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchChannelNewFollowerNotificationHandlerTests : TestBase
{
    private readonly IChannelFollowCount _channelFollowCount;
    private readonly IFollowerMilestone _followerMilestone;
    private readonly INotificationHandler<TwitchChannelNewFollower> _notificationHandler;

    public TwitchChannelNewFollowerNotificationHandlerTests()
    {
        this._channelFollowCount = GetSubstitute<IChannelFollowCount>();
        this._followerMilestone = GetSubstitute<IFollowerMilestone>();

        this._notificationHandler = new TwitchChannelNewFollowerNotificationHandler(channelFollowCount: this._channelFollowCount,
                                                                                    followerMilestone: this._followerMilestone,
                                                                                    this.GetTypedLogger<TwitchChannelNewFollowerNotificationHandler>());
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(2, false)]
    [InlineData(120, false)]
    [InlineData(0, true)]
    [InlineData(2, true)]
    [InlineData(120, true)]
    public async Task HandleAsync(int followerCount, bool streamOnline)
    {
        this._channelFollowCount.GetCurrentFollowerCountAsync(streamer: MockReferenceData.Streamer, Arg.Any<CancellationToken>())
            .Returns(followerCount);

        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer,
                                                   user: MockReferenceData.Viewer,
                                                   streamOnline: streamOnline,
                                                   isStreamer: false,
                                                   accountCreated: DateTime.MinValue,
                                                   followCount: 42),
                                               cancellationToken: CancellationToken.None);

        await this.ReceivedGetCurrentFollowerCountAsync();

        await this.ReceivedIssueMilestoneUpdateAsync(followerCount);
    }

    [Fact]
    public async Task HandleExceptionAsync()
    {
        const bool streamOnline = false;
        this._channelFollowCount.GetCurrentFollowerCountAsync(streamer: MockReferenceData.Streamer, Arg.Any<CancellationToken>())
            .Throws<TimeoutException>();

        await this._notificationHandler.Handle(new(streamer: MockReferenceData.Streamer,
                                                   user: MockReferenceData.Viewer,
                                                   streamOnline: streamOnline,
                                                   isStreamer: false,
                                                   accountCreated: DateTime.MinValue,
                                                   followCount: 42),
                                               cancellationToken: CancellationToken.None);

        await this.ReceivedGetCurrentFollowerCountAsync();

        await this.DidNotReceiveIssueMilestoneUpdateAsync();
    }

    private Task ReceivedIssueMilestoneUpdateAsync(int followerCount)
    {
        return this._followerMilestone.Received(1)
                   .IssueMilestoneUpdateAsync(streamer: MockReferenceData.Streamer, followers: followerCount, Arg.Any<CancellationToken>());
    }

    private Task DidNotReceiveIssueMilestoneUpdateAsync()
    {
        return this._followerMilestone.DidNotReceive()
                   .IssueMilestoneUpdateAsync(Arg.Any<Streamer>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    private Task ReceivedGetCurrentFollowerCountAsync()
    {
        return this._channelFollowCount.Received(1)
                   .GetCurrentFollowerCountAsync(streamer: MockReferenceData.Streamer, Arg.Any<CancellationToken>());
    }
}