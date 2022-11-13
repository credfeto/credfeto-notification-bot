using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Actions.Services;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using FunFair.Test.Common;
using Mediator;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Actions.Services;

public sealed class FollowerMilestoneTests : TestBase
{
    private readonly IFollowerMilestone _followerMileStone;
    private readonly IMediator _mediator;
    private readonly ITwitchStreamDataManager _twitchStreamDataManager;

    public FollowerMilestoneTests()
    {
        this._twitchStreamDataManager = GetSubstitute<ITwitchStreamDataManager>();
        this._mediator = GetSubstitute<IMediator>();

        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions(authentication: MockReferenceData.TwitchAuthentication,
                                                   Array.Empty<TwitchModChannel>(),
                                                   heists: MockReferenceData.Heists,
                                                   marbles: Array.Empty<TwitchMarbles>(),
                                                   ignoredUsers: MockReferenceData.IgnoredUsers,
                                                   milestones: MockReferenceData.TwitchMilestones));

        this._followerMileStone = new FollowerMilestone(options: options,
                                                        mediator: this._mediator,
                                                        twitchStreamDataManager: this._twitchStreamDataManager,
                                                        this.GetTypedLogger<FollowerMilestone>());
    }

    [Fact]
    public async Task FollowerMileStoneReachedNewStreamerZeroAsync()
    {
        this._twitchStreamDataManager.UpdateFollowerMilestoneAsync(streamer: MockReferenceData.Streamer, followerCount: 1)
            .Returns(true);

        await this._followerMileStone.IssueMilestoneUpdateAsync(streamer: MockReferenceData.Streamer, followers: 1, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();
        await this.DidNotReceiveUpdateMilestoneAsync();
    }

    [Fact]
    public async Task FollowerMileStoneReachedForTheFirstTimeAsync()
    {
        this._twitchStreamDataManager.UpdateFollowerMilestoneAsync(streamer: MockReferenceData.Streamer, followerCount: 100)
            .Returns(true);

        await this._followerMileStone.IssueMilestoneUpdateAsync(streamer: MockReferenceData.Streamer, followers: 101, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync(milestone: 100, nextMilestone: 1000);
        await this.ReceivedUpdateMilestoneAsync(100);
    }

    [Fact]
    public async Task FollowerMileStoneReachedButAlreadyMetTimeAsync()
    {
        this._twitchStreamDataManager.UpdateFollowerMilestoneAsync(streamer: MockReferenceData.Streamer, followerCount: 100)
            .Returns(false);

        await this._followerMileStone.IssueMilestoneUpdateAsync(streamer: MockReferenceData.Streamer, followers: 101, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();
        await this.ReceivedUpdateMilestoneAsync(100);
    }

    private Task ReceivedUpdateMilestoneAsync(int followerCount)
    {
        return this._twitchStreamDataManager.Received(1)
                   .UpdateFollowerMilestoneAsync(streamer: MockReferenceData.Streamer, followerCount: followerCount);
    }

    private Task DidNotReceiveUpdateMilestoneAsync()
    {
        return this._twitchStreamDataManager.DidNotReceive()
                   .UpdateFollowerMilestoneAsync(Arg.Any<Streamer>(), Arg.Any<int>());
    }

    private ValueTask ReceivedPublishMessageAsync(int milestone, int nextMilestone)
    {
        return this._mediator.Received(1)
                   .Publish(Arg.Is<TwitchFollowerMilestoneReached>(t => t.Streamer == MockReferenceData.Streamer && t.MilestoneReached == milestone &&
                                                                        t.NextMilestone == nextMilestone),
                            Arg.Any<CancellationToken>());
    }

    private ValueTask DidNotReceivePublishMessageAsync()
    {
        return this._mediator.DidNotReceive()
                   .Publish(Arg.Any<TwitchFollowerMilestoneReached>(), Arg.Any<CancellationToken>());
    }
}