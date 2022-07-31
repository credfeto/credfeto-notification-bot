using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Actions.Services;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.StreamState;
using FunFair.Test.Common;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Actions.Services;

public sealed class ShoutoutJoinerTests : TestBase
{
    private static readonly Streamer StreamerShoutOutsEnabled = MockReferenceData.Streamer.Next();
    private static readonly Streamer StreamerShoutOutsDisabled = MockReferenceData.Streamer.Next();
    private static readonly Streamer StreamerShoutOutsException = MockReferenceData.Streamer.Next();
    private static readonly Viewer VisitingChannelFriendWithMessage = MockReferenceData.Viewer.Next();
    private static readonly Viewer VisitingChannelFriendWithNoMessage = Viewer.FromString(nameof(VisitingChannelFriendWithNoMessage));
    private static readonly Viewer VisitingChannel = Viewer.FromString(nameof(VisitingChannel));
    private readonly IShoutoutJoiner _shoutoutJoiner;
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    public ShoutoutJoinerTests()
    {
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions(authentication: MockReferenceData.TwitchAuthentication,
                                                   milestones: MockReferenceData.TwitchMilestones,
                                                   ignoredUsers: MockReferenceData.IgnoredUsers,
                                                   heists: MockReferenceData.Heists,
                                                   marbles: null,
                                                   channels: new()
                                                             {
                                                                 new(StreamerShoutOutsEnabled.ToString(),
                                                                     new(enabled: true,
                                                                         new()
                                                                         {
                                                                             new(VisitingChannelFriendWithMessage.ToString(),
                                                                                 message: "Check out this weird and wonderful streamer!"),
                                                                             new(VisitingChannelFriendWithNoMessage.ToString(), message: null)
                                                                         }),
                                                                     raids: MockReferenceData.TwitchChannelRaids,
                                                                     thanks: MockReferenceData.TwitchChannelThanks,
                                                                     mileStones: MockReferenceData.TwitchChanelMileStone,
                                                                     welcome: MockReferenceData.TwitchChannelWelcome)
                                                             }));

        this._twitchChatMessageChannel = GetSubstitute<IMessageChannel<TwitchChatMessage>>();

        ITwitchChannelManager twitchChannelManager = GetSubstitute<ITwitchChannelManager>();
        ITwitchChannelState shoutOutsEnabled = GetSubstitute<ITwitchChannelState>();
        shoutOutsEnabled.Settings.ShoutOutsEnabled.Returns(true);
        twitchChannelManager.GetStreamer(StreamerShoutOutsEnabled)
                            .Returns(shoutOutsEnabled);
        ITwitchChannelState shoutOutsDisabled = GetSubstitute<ITwitchChannelState>();
        shoutOutsDisabled.Settings.ShoutOutsEnabled.Returns(false);
        twitchChannelManager.GetStreamer(StreamerShoutOutsDisabled)
                            .Returns(shoutOutsDisabled);

        twitchChannelManager.GetStreamer(StreamerShoutOutsException)
                            .Throws<TimeoutException>();

        this._shoutoutJoiner = new ShoutoutJoiner(options: options,
                                                  twitchChannelManager: twitchChannelManager,
                                                  twitchChatMessageChannel: this._twitchChatMessageChannel,
                                                  this.GetTypedLogger<ShoutoutJoiner>());
    }

    private ValueTask ReceivedPublishMessageAsync(string expectedMessage)
    {
        return this._twitchChatMessageChannel.Received(1)
                   .PublishAsync(Arg.Is<TwitchChatMessage>(t => t.Streamer == StreamerShoutOutsEnabled && t.Message == expectedMessage), Arg.Any<CancellationToken>());
    }

    private ValueTask DidNotReceivePublishMessageAsync()
    {
        return this._twitchChatMessageChannel.DidNotReceive()
                   .PublishAsync(Arg.Any<TwitchChatMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task IssueShoutoutUnknownStreamerAsync()
    {
        TwitchUser visitingStreamer = new(id: "123456", userName: VisitingChannel, isStreamer: true, new(year: 2020, month: 1, day: 1));

        await this._shoutoutJoiner.IssueShoutoutAsync(streamer: StreamerShoutOutsEnabled,
                                                      visitingStreamer: visitingStreamer,
                                                      isRegular: false,
                                                      cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();
    }

    [Fact]
    public async Task IssueShoutoutDisabledAsync()
    {
        TwitchUser visitingStreamer = new(id: "123456", userName: VisitingChannel, isStreamer: true, new(year: 2020, month: 1, day: 1));

        await this._shoutoutJoiner.IssueShoutoutAsync(streamer: StreamerShoutOutsDisabled,
                                                      visitingStreamer: visitingStreamer,
                                                      isRegular: true,
                                                      cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();
    }

    [Fact]
    public async Task IssueShoutoutRegularStreamerVisitorAsync()
    {
        TwitchUser visitingStreamer = new(id: "123456", userName: VisitingChannel, isStreamer: true, new(year: 2020, month: 1, day: 1));

        await this._shoutoutJoiner.IssueShoutoutAsync(streamer: StreamerShoutOutsEnabled,
                                                      visitingStreamer: visitingStreamer,
                                                      isRegular: true,
                                                      cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"!so @{VisitingChannel}");
    }

    [Fact]
    public async Task IssueShoutoutExceptionAsync()
    {
        TwitchUser visitingStreamer = new(id: "123456", userName: VisitingChannel, isStreamer: true, new(year: 2020, month: 1, day: 1));

        await this._shoutoutJoiner.IssueShoutoutAsync(streamer: StreamerShoutOutsException,
                                                      visitingStreamer: visitingStreamer,
                                                      isRegular: true,
                                                      cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();
    }

    [Fact]
    public async Task IssueShoutoutFriendStreamerVisitorWithNoCustomMessageAsync()
    {
        TwitchUser visitingStreamer = new(id: "123456", userName: VisitingChannelFriendWithNoMessage, isStreamer: true, new(year: 2020, month: 1, day: 1));

        await this._shoutoutJoiner.IssueShoutoutAsync(streamer: StreamerShoutOutsEnabled,
                                                      visitingStreamer: visitingStreamer,
                                                      isRegular: true,
                                                      cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"!so @{VisitingChannelFriendWithNoMessage}");
    }

    [Fact]
    public async Task IssueShoutoutFriendStreamerVisitorWithCustomMessageAsync()
    {
        TwitchUser visitingStreamer = new(id: "123456", userName: VisitingChannelFriendWithMessage, isStreamer: true, new(year: 2020, month: 1, day: 1));

        await this._shoutoutJoiner.IssueShoutoutAsync(streamer: StreamerShoutOutsEnabled,
                                                      visitingStreamer: visitingStreamer,
                                                      isRegular: true,
                                                      cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync("Check out this weird and wonderful streamer!");
    }
}