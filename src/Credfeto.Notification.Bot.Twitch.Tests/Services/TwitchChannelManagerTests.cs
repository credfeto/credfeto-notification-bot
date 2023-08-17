using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using Mediator;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Services;

public sealed class TwitchChannelManagerTests : TestBase
{
    private static readonly Streamer Guest1 = MockReferenceData.Streamer.Next();
    private static readonly Streamer Guest2 = MockReferenceData.Streamer.Next();

    private static readonly DateTime StreamStartDate = new(year: 2020, month: 1, day: 1);

    private readonly IMediator _mediator;
    private readonly ITwitchChannelManager _twitchChannelManager;
    private readonly ITwitchStreamDataManager _twitchStreamerDataManager;
    private readonly IUserInfoService _userInfoService;

    public TwitchChannelManagerTests()
    {
        this._userInfoService = GetSubstitute<IUserInfoService>();
        this._twitchStreamerDataManager = GetSubstitute<ITwitchStreamDataManager>();
        this._mediator = GetSubstitute<IMediator>();

        int[] followerMilestones =
        {
            10,
            20,
            30
        };

        int[] subscriberMilestones =
        {
            10,
            20,
            30
        };

        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions(authentication: MockReferenceData.TwitchAuthentication,
                                                   ignoredUsers: new[]
                                                                 {
                                                                     MockReferenceData.Ignored.Value
                                                                 },
                                                   milestones: new(followers: followerMilestones, subscribers: subscriberMilestones),
                                                   chatCommands: Array.Empty<TwitchChatCommand>(),
                                                   channels: new TwitchModChannel[]
                                                             {
                                                                 new(channelName: ((Streamer)MockReferenceData.Streamer).Value,
                                                                     welcome: new(enabled: true),
                                                                     mileStones: new(enabled: true),
                                                                     raids: new(enabled: true, immediate: null, calmDown: null),
                                                                     thanks: new(enabled: true),
                                                                     shoutOuts: new(enabled: true, new() { new(channel: Guest1.Value, message: "!guest"), new(channel: Guest2.Value, message: null) }))
                                                             }));

        this._twitchChannelManager = new TwitchChannelManager(options: options,
                                                              userInfoService: this._userInfoService,
                                                              twitchStreamDataManager: this._twitchStreamerDataManager,
                                                              mediator: this._mediator,
                                                              this.GetTypedLogger<TwitchChannelManager>());
    }

    [Fact]
    public async Task StreamOnlineOfflineAsync()
    {
        ITwitchChannelState twitchChannelState = await this.GetOnlineStreamAsync();

        await this._twitchStreamerDataManager.Received(1)
                  .RecordStreamStartAsync(streamer: MockReferenceData.Streamer, streamStartDate: StreamStartDate);

        twitchChannelState.Offline();

        Assert.False(condition: twitchChannelState.IsOnline, userMessage: "Should be offline");
    }

    [Fact]
    public async Task StreamRaidedWhenOnlineAsync()
    {
        ITwitchChannelState twitchChannelState = await this.GetOnlineStreamAsync();

        await twitchChannelState.RaidedAsync(Guest1.ToViewer(), viewerCount: 12, cancellationToken: CancellationToken.None);

        await this._mediator.Received(1)
                  .Publish(Arg.Is<TwitchStreamRaided>(t => t.Streamer == MockReferenceData.Streamer && t.Raider == Guest1.ToViewer() && t.ViewerCount == 12),
                           cancellationToken: CancellationToken.None);
    }

    [Fact]
    public async Task StreamRaidedWhenOfflineAsync()
    {
        ITwitchChannelState twitchChannelState = this.GetOfflineStream();

        await twitchChannelState.RaidedAsync(Guest1.ToViewer(), viewerCount: 12, cancellationToken: CancellationToken.None);

        await this._mediator.DidNotReceive()
                  .Publish(Arg.Is<TwitchStreamRaided>(t => t.Streamer == MockReferenceData.Streamer && t.Raider == Guest1.ToViewer() && t.ViewerCount == 12),
                           cancellationToken: CancellationToken.None);
    }

    private ITwitchChannelState GetOfflineStream()
    {
        ITwitchChannelState twitchChannelState = this._twitchChannelManager.GetStreamer(MockReferenceData.Streamer);

        Assert.False(condition: twitchChannelState.IsOnline, userMessage: "Should be offline");

        return twitchChannelState;
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task FirstChatMessageWhenOnlineAsync(bool isRegular)
    {
        ITwitchChannelState twitchChannelState = await this.GetOnlineStreamAsync();

        this.MockIsRegularChatter(isRegular);
        this.MockIsFirstMessageInStream(true);

        await SendChatMessageAsync(twitchChannelState);

        await this.DidNotReceiveBitGiftNotificationAsync();
        await this.ReceivedCheckForIsRegularChatterAsync();
        await this.ReceivedCheckForFirstMessageInStreamAsync();

        await this._mediator.Received(1)
                  .Publish(Arg.Is<TwitchStreamNewChatter>(t => t.Streamer == MockReferenceData.Streamer && t.User == Guest1.ToViewer() && t.IsRegular == isRegular),
                           cancellationToken: CancellationToken.None);
    }

    [Fact]
    public async Task ChatMessageWhenOfflineAsync()
    {
        ITwitchChannelState twitchChannelState = this.GetOfflineStream();

        await SendChatMessageAsync(twitchChannelState);

        await this.DidNotReceiveBitGiftNotificationAsync();
        await this.DidNotReceiveCheckForIsRegularChatterAsync();
        await this.DidNotReceiveCheckForFirstMessageInStreamAsync();

        await this._mediator.DidNotReceive()
                  .Publish(Arg.Any<TwitchStreamNewChatter>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task SubsequentChatMessageWhenOnlineAsync(bool isRegular)
    {
        ITwitchChannelState twitchChannelState = await this.GetOnlineStreamAsync();

        this.MockIsRegularChatter(isRegular);
        this.MockIsFirstMessageInStream(false);

        await SendChatMessageAsync(twitchChannelState);

        await this.DidNotReceiveBitGiftNotificationAsync();
        await this.DidNotReceiveCheckForIsRegularChatterAsync();
        await this.ReceivedCheckForFirstMessageInStreamAsync();

        await this._mediator.DidNotReceive()
                  .Publish(Arg.Is<TwitchStreamNewChatter>(t => t.Streamer == MockReferenceData.Streamer && t.User == Guest1.ToViewer() && t.IsRegular == isRegular),
                           cancellationToken: CancellationToken.None);
    }

    [Fact]
    public async Task BitsGiftWhenOnlineAsync()
    {
        ITwitchChannelState twitchChannelState = await this.GetOnlineStreamAsync();

        await twitchChannelState.BitsGiftedAsync(Guest1.ToViewer(), bits: 124, cancellationToken: CancellationToken.None);

        await this.ReceivedBitGiftNotificationAsync(Guest1.ToViewer(), amount: 124);
    }

    [Fact]
    public async Task BitsGiftWhenOfflineAsync()
    {
        ITwitchChannelState twitchChannelState = this.GetOfflineStream();

        await twitchChannelState.BitsGiftedAsync(Guest1.ToViewer(), bits: 124, cancellationToken: CancellationToken.None);

        await this.DidNotReceiveBitGiftNotificationAsync();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task FirstChatMessageByIgnoredUserWhenOnlineAsync(bool isRegular)
    {
        ITwitchChannelState twitchChannelState = await this.GetOnlineStreamAsync();

        this.MockIsRegularChatter(isRegular);
        this.MockIsFirstMessageInStream(true);

        await twitchChannelState.ChatMessageAsync(user: MockReferenceData.Ignored, message: "Hello world", cancellationToken: CancellationToken.None);

        await this.DidNotReceiveBitGiftNotificationAsync();
        await this.DidNotReceiveCheckForIsRegularChatterAsync();
        await this.DidNotReceiveCheckForFirstMessageInStreamAsync();

        await this._mediator.DidNotReceive()
                  .Publish(Arg.Is<TwitchStreamNewChatter>(t => t.Streamer == MockReferenceData.Streamer && t.User == Guest1.ToViewer() && t.IsRegular == isRegular),
                           cancellationToken: CancellationToken.None);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task FirstChatMessageByStreamerUserWhenOnlineAsync(bool isRegular)
    {
        ITwitchChannelState twitchChannelState = await this.GetOnlineStreamAsync();

        this.MockIsRegularChatter(isRegular);
        this.MockIsFirstMessageInStream(true);

        await twitchChannelState.ChatMessageAsync(((Streamer)MockReferenceData.Streamer).ToViewer(), message: "Hello world", cancellationToken: CancellationToken.None);

        await this.DidNotReceiveBitGiftNotificationAsync();
        await this.DidNotReceiveCheckForIsRegularChatterAsync();
        await this.DidNotReceiveCheckForFirstMessageInStreamAsync();

        await this._mediator.DidNotReceive()
                  .Publish(Arg.Is<TwitchStreamNewChatter>(t => t.Streamer == MockReferenceData.Streamer && t.User == Guest1.ToViewer() && t.IsRegular == isRegular),
                           cancellationToken: CancellationToken.None);
    }

    private ValueTask DidNotReceiveBitGiftNotificationAsync()
    {
        return this._mediator.DidNotReceive()
                   .Publish(Arg.Is<TwitchBitsGift>(t => t.Streamer == MockReferenceData.Streamer && t.User == Guest1.ToViewer()), cancellationToken: CancellationToken.None);
    }

    private ValueTask ReceivedBitGiftNotificationAsync(Viewer viewer, int amount)
    {
        return this._mediator.Received(1)
                   .Publish(Arg.Is<TwitchBitsGift>(t => t.Streamer == MockReferenceData.Streamer && t.User == viewer && t.Bits == amount), cancellationToken: CancellationToken.None);
    }

    private Task ReceivedCheckForFirstMessageInStreamAsync()
    {
        return this._twitchStreamerDataManager.Received(1)
                   .IsFirstMessageInStreamAsync(streamer: MockReferenceData.Streamer, streamStartDate: StreamStartDate, Guest1.ToViewer());
    }

    private Task DidNotReceiveCheckForFirstMessageInStreamAsync()
    {
        return this._twitchStreamerDataManager.DidNotReceive()
                   .IsFirstMessageInStreamAsync(Arg.Any<Streamer>(), Arg.Any<DateTimeOffset>(), Arg.Any<Viewer>());
    }

    private Task ReceivedCheckForIsRegularChatterAsync()
    {
        return this._twitchStreamerDataManager.Received(1)
                   .IsRegularChatterAsync(streamer: MockReferenceData.Streamer, Guest1.ToViewer());
    }

    private Task DidNotReceiveCheckForIsRegularChatterAsync()
    {
        return this._twitchStreamerDataManager.DidNotReceive()
                   .IsRegularChatterAsync(Arg.Any<Streamer>(), Arg.Any<Viewer>());
    }

    private static ValueTask SendChatMessageAsync(ITwitchChannelState twitchChannelState)
    {
        return twitchChannelState.ChatMessageAsync(Guest1.ToViewer(), message: "Hello world", cancellationToken: CancellationToken.None);
    }

    private async Task<ITwitchChannelState> GetOnlineStreamAsync()
    {
        ITwitchChannelState twitchChannelState = this._twitchChannelManager.GetStreamer(MockReferenceData.Streamer);

        await twitchChannelState.OnlineAsync(gameName: "FunGame", startDate: StreamStartDate);

        Assert.True(condition: twitchChannelState.IsOnline, userMessage: "Should be online");

        return twitchChannelState;
    }

    private void MockIsFirstMessageInStream(bool isFirstMessage)
    {
        this._twitchStreamerDataManager.IsFirstMessageInStreamAsync(streamer: MockReferenceData.Streamer, streamStartDate: StreamStartDate, Guest1.ToViewer())
            .Returns(isFirstMessage);
    }

    private void MockIsRegularChatter(bool isRegularChatter)
    {
        this._twitchStreamerDataManager.IsRegularChatterAsync(streamer: MockReferenceData.Streamer, Guest1.ToViewer())
            .Returns(isRegularChatter);
    }
}