using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using MediatR;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Services;

public sealed class TwitchChannelManagerTests : TestBase
{
    private static readonly Streamer Streamer = Streamer.FromString(nameof(Streamer));
    private static readonly Streamer Guest1 = Streamer.FromString(nameof(Guest1));
    private static readonly Streamer Guest2 = Streamer.FromString(nameof(Guest2));
    private static readonly Viewer Ignored = Viewer.FromString(nameof(Ignored));

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

        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions
                              {
                                  Authentication = new() { ClientId = "Invalid", ClientSecret = "Invalid", ClientAccessToken = "Invalid" },
                                  IgnoredUsers = new() { Ignored.Value },
                                  Milestones = new() { Followers = new() { 10, 20, 30 }, Subscribers = new() { 10, 20, 30 } },
                                  Channels = new()
                                             {
                                                 new()
                                                 {
                                                     ChannelName = Streamer.Value,
                                                     Welcome = new() { Enabled = true },
                                                     MileStones = new() { Enabled = true },
                                                     Raids = new() { Enabled = true },
                                                     Thanks = new() { Enabled = true },
                                                     ShoutOuts = new()
                                                                 {
                                                                     Enabled = true,
                                                                     FriendChannels = new()
                                                                                      {
                                                                                          new() { Channel = Guest1.Value, Message = "!guest" }, new() { Channel = Guest2.Value, Message = null }
                                                                                      }
                                                                 }
                                                 }
                                             }
                              });

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
                  .RecordStreamStartAsync(streamer: Streamer, streamStartDate: StreamStartDate);

        twitchChannelState.Offline();

        Assert.False(condition: twitchChannelState.IsOnline, userMessage: "Should be offline");
    }

    [Fact]
    public async Task StreamRaidedWhenOnlineAsync()
    {
        ITwitchChannelState twitchChannelState = await this.GetOnlineStreamAsync();

        await twitchChannelState.RaidedAsync(Guest1.ToViewer(), viewerCount: 12, cancellationToken: CancellationToken.None);

        await this._mediator.Received(1)
                  .Publish(Arg.Is<TwitchStreamRaided>(t => t.Streamer == Streamer && t.Raider == Guest1.ToViewer() && t.ViewerCount == 12), cancellationToken: CancellationToken.None);
    }

    [Fact]
    public async Task StreamRaidedWhenOfflineAsync()
    {
        ITwitchChannelState twitchChannelState = this._twitchChannelManager.GetStreamer(Streamer);

        Assert.False(condition: twitchChannelState.IsOnline, userMessage: "Should be offline");

        await twitchChannelState.RaidedAsync(Guest1.ToViewer(), viewerCount: 12, cancellationToken: CancellationToken.None);

        await this._mediator.DidNotReceive()
                  .Publish(Arg.Is<TwitchStreamRaided>(t => t.Streamer == Streamer && t.Raider == Guest1.ToViewer() && t.ViewerCount == 12), cancellationToken: CancellationToken.None);
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
                  .Publish(Arg.Is<TwitchStreamNewChatter>(t => t.Streamer == Streamer && t.User == Guest1.ToViewer() && t.IsRegular == isRegular), cancellationToken: CancellationToken.None);
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
                  .Publish(Arg.Is<TwitchStreamNewChatter>(t => t.Streamer == Streamer && t.User == Guest1.ToViewer() && t.IsRegular == isRegular), cancellationToken: CancellationToken.None);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task FirstChatMessageByIgnoredUserWhenOnlineAsync(bool isRegular)
    {
        ITwitchChannelState twitchChannelState = await this.GetOnlineStreamAsync();

        this.MockIsRegularChatter(isRegular);
        this.MockIsFirstMessageInStream(true);

        await twitchChannelState.ChatMessageAsync(user: Ignored, message: "Hello world", bits: 0, cancellationToken: CancellationToken.None);

        await this.DidNotReceiveBitGiftNotificationAsync();
        await this.DidNotReceiveCheckForIsRegularChatterAsync();
        await this.DidNotReceiveCheckForFirstMessageInStreamAsync();

        await this._mediator.DidNotReceive()
                  .Publish(Arg.Is<TwitchStreamNewChatter>(t => t.Streamer == Streamer && t.User == Guest1.ToViewer() && t.IsRegular == isRegular), cancellationToken: CancellationToken.None);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task FirstChatMessageByStreamerUserWhenOnlineAsync(bool isRegular)
    {
        ITwitchChannelState twitchChannelState = await this.GetOnlineStreamAsync();

        this.MockIsRegularChatter(isRegular);
        this.MockIsFirstMessageInStream(true);

        await twitchChannelState.ChatMessageAsync(Streamer.ToViewer(), message: "Hello world", bits: 0, cancellationToken: CancellationToken.None);

        await this.DidNotReceiveBitGiftNotificationAsync();
        await this.DidNotReceiveCheckForIsRegularChatterAsync();
        await this.DidNotReceiveCheckForFirstMessageInStreamAsync();

        await this._mediator.DidNotReceive()
                  .Publish(Arg.Is<TwitchStreamNewChatter>(t => t.Streamer == Streamer && t.User == Guest1.ToViewer() && t.IsRegular == isRegular), cancellationToken: CancellationToken.None);
    }

    private Task DidNotReceiveBitGiftNotificationAsync()
    {
        return this._mediator.DidNotReceive()
                   .Publish(Arg.Is<TwitchBitsGift>(t => t.Streamer == Streamer && t.User == Guest1.ToViewer()), cancellationToken: CancellationToken.None);
    }

    private Task ReceivedCheckForFirstMessageInStreamAsync()
    {
        return this._twitchStreamerDataManager.Received(1)
                   .IsFirstMessageInStreamAsync(streamer: Streamer, streamStartDate: StreamStartDate, Guest1.ToViewer());
    }

    private Task DidNotReceiveCheckForFirstMessageInStreamAsync()
    {
        return this._twitchStreamerDataManager.DidNotReceive()
                   .IsFirstMessageInStreamAsync(Arg.Any<Streamer>(), Arg.Any<DateTime>(), Arg.Any<Viewer>());
    }

    private Task ReceivedCheckForIsRegularChatterAsync()
    {
        return this._twitchStreamerDataManager.Received(1)
                   .IsRegularChatterAsync(streamer: Streamer, Guest1.ToViewer());
    }

    private Task DidNotReceiveCheckForIsRegularChatterAsync()
    {
        return this._twitchStreamerDataManager.DidNotReceive()
                   .IsRegularChatterAsync(Arg.Any<Streamer>(), Arg.Any<Viewer>());
    }

    private static Task SendChatMessageAsync(ITwitchChannelState twitchChannelState)
    {
        return twitchChannelState.ChatMessageAsync(Guest1.ToViewer(), message: "Hello world", bits: 0, cancellationToken: CancellationToken.None);
    }

    private async Task<ITwitchChannelState> GetOnlineStreamAsync()
    {
        ITwitchChannelState twitchChannelState = this._twitchChannelManager.GetStreamer(Streamer);

        await twitchChannelState.OnlineAsync(gameName: "FunGame", startDate: StreamStartDate);

        Assert.True(condition: twitchChannelState.IsOnline, userMessage: "Should be online");

        return twitchChannelState;
    }

    private void MockIsFirstMessageInStream(bool isFirstMessage)
    {
        this._twitchStreamerDataManager.IsFirstMessageInStreamAsync(streamer: Streamer, streamStartDate: StreamStartDate, Guest1.ToViewer())
            .Returns(isFirstMessage);
    }

    private void MockIsRegularChatter(bool isRegularChatter)
    {
        this._twitchStreamerDataManager.IsRegularChatterAsync(streamer: Streamer, Guest1.ToViewer())
            .Returns(isRegularChatter);
    }

    /*
 *     Task ChatMessageAsync(Viewer user, string message, int bits, CancellationToken cancellationToken);

    Task GiftedMultipleAsync(Viewer giftedBy, int count, string months, in CancellationToken cancellationToken);

    Task GiftedSubAsync(Viewer giftedBy, string months, in CancellationToken cancellationToken);

    Task ContinuedSubAsync(Viewer user, in CancellationToken cancellationToken);

    Task PrimeToPaidAsync(Viewer user, in CancellationToken cancellationToken);

    Task NewSubscriberPaidAsync(Viewer user, in CancellationToken cancellationToken);

    Task NewSubscriberPrimeAsync(Viewer user, in CancellationToken cancellationToken);

    Task ResubscribePaidAsync(Viewer user, int months, in CancellationToken cancellationToken);

    Task ResubscribePrimeAsync(Viewer user, int months, in CancellationToken cancellationToken);

    Task NewFollowerAsync(Viewer user, CancellationToken cancellationToken);
 */
}