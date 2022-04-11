using System;
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

public sealed class TwitchChannelNewFollowerNotificationHandlerTests : TestBase
{
    private static readonly Channel Channel = Types.ChannelFromString(nameof(Channel));
    private static readonly User User = Types.UserFromString(nameof(User));

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
        this._channelFollowCount.GetCurrentFollowerCountAsync(channel: Channel, Arg.Any<CancellationToken>())
            .Returns(followerCount);

        await this._notificationHandler.Handle(new(channel: Channel, user: User, streamOnline: streamOnline, isStreamer: false, accountCreated: DateTime.MinValue, followCount: 42),
                                               cancellationToken: CancellationToken.None);

        await this.ReceivedGetCurrentFollowerCountAsync();

        await this.ReceivedIssueMilestoneUpdateAsync(followerCount);
    }

    private Task ReceivedIssueMilestoneUpdateAsync(int followerCount)
    {
        return this._followerMilestone.Received(1)
                   .IssueMilestoneUpdateAsync(channel: Channel, followers: followerCount, Arg.Any<CancellationToken>());
    }

    private Task ReceivedGetCurrentFollowerCountAsync()
    {
        return this._channelFollowCount.Received(1)
                   .GetCurrentFollowerCountAsync(channel: Channel, Arg.Any<CancellationToken>());
    }
}