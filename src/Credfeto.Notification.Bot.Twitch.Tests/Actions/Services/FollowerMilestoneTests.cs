using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Actions.Services;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.StreamState;
using FunFair.Test.Common;
using MediatR;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Actions.Services;

public sealed class FollowerMilestoneTests : TestBase
{
    private const string CHANNEL = nameof(CHANNEL);
    private readonly IFollowerMilestone _followerMileStone;
    private readonly IMediator _mediator;
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;
    private readonly ITwitchStreamDataManager _twitchStreamDataManager;

    public FollowerMilestoneTests()
    {
        this._twitchChatMessageChannel = GetSubstitute<IMessageChannel<TwitchChatMessage>>();
        this._twitchStreamDataManager = GetSubstitute<ITwitchStreamDataManager>();
        this._mediator = GetSubstitute<IMediator>();

        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions
                              {
                                  Milestones = new()
                                               {
                                                   Followers = new()
                                                               {
                                                                   1,
                                                                   10,
                                                                   100,
                                                                   1000,
                                                                   2000,
                                                                   3000,
                                                                   4000
                                                               }
                                               }
                              });

        this._followerMileStone = new FollowerMilestone(options: options,
                                                        mediator: this._mediator,
                                                        twitchStreamDataManager: this._twitchStreamDataManager,
                                                        twitchChatMessageChannel: this._twitchChatMessageChannel,
                                                        this.GetTypedLogger<FollowerMilestone>());
    }

    [Fact]
    public async Task FollowerMileStoneReachedForTheFirstTimeAsync()
    {
        this._twitchStreamDataManager.UpdateFollowerMilestoneAsync(channel: CHANNEL, followerCount: 100)
            .Returns(true);

        await this._followerMileStone.IssueMilestoneUpdateAsync(channel: CHANNEL, followers: 101, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"/me @{CHANNEL} Woo! {100} followers reached!");
        await this.ReceivedUpdateMilestoneAsync();
    }

    [Fact]
    public async Task FollowerMileStoneReachedButAlreadyMetTimeAsync()
    {
        this._twitchStreamDataManager.UpdateFollowerMilestoneAsync(channel: CHANNEL, followerCount: 100)
            .Returns(false);

        await this._followerMileStone.IssueMilestoneUpdateAsync(channel: CHANNEL, followers: 101, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();
        await this.ReceivedUpdateMilestoneAsync();
    }

    private Task<bool> ReceivedUpdateMilestoneAsync()
    {
        return this._twitchStreamDataManager.Received(1)
                   .UpdateFollowerMilestoneAsync(channel: CHANNEL, followerCount: 100);
    }

    private ValueTask ReceivedPublishMessageAsync(string expectedMessage)
    {
        return this._twitchChatMessageChannel.Received(1)
                   .PublishAsync(Arg.Is<TwitchChatMessage>(t => t.Channel == CHANNEL && t.Message == expectedMessage), Arg.Any<CancellationToken>());
    }

    private ValueTask DidNotReceivePublishMessageAsync()
    {
        return this._twitchChatMessageChannel.DidNotReceive()
                   .PublishAsync(Arg.Any<TwitchChatMessage>(), Arg.Any<CancellationToken>());
    }
}