using System.Threading;
using System.Threading.Tasks;
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
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Actions.Services;

public sealed class ContributionThanksTests : TestBase
{
    private static readonly Streamer Streamer = Streamer.FromString(nameof(Streamer));
    private static readonly Viewer GiftingUser = Viewer.FromString(nameof(GiftingUser));
    private static readonly Viewer User = Viewer.FromString(nameof(User));

    private readonly IContributionThanks _contributionThanks;
    private readonly ICurrentTimeSource _currentTimeSource;
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    public ContributionThanksTests()
    {
        this._twitchChatMessageChannel = GetSubstitute<IMessageChannel<TwitchChatMessage>>();
        this._currentTimeSource = GetSubstitute<ICurrentTimeSource>();
        IOptions<TwitchBotOptions> options = Substitute.For<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions { Channels = new() { new() { ChannelName = Streamer.ToString(), Thanks = new() { Enabled = true } } } });

        ITwitchChannelManager twitchChannelManager = GetSubstitute<ITwitchChannelManager>();
        ITwitchChannelState twitchChannelState = GetSubstitute<ITwitchChannelState>();
        twitchChannelManager.GetStreamer(Arg.Any<Streamer>())
                            .Returns(twitchChannelState);
        twitchChannelState.Settings.ThanksEnabled.Returns(true);

        this._contributionThanks = new ContributionThanks(twitchChannelManager: twitchChannelManager,
                                                          twitchChatMessageChannel: this._twitchChatMessageChannel,
                                                          currentTimeSource: this._currentTimeSource,
                                                          this.GetTypedLogger<ContributionThanks>());
    }

    private ValueTask ReceivedPublishMessageAsync(string expectedMessage)
    {
        return this._twitchChatMessageChannel.Received(1)
                   .PublishAsync(Arg.Is<TwitchChatMessage>(t => t.Streamer == Streamer && t.Message == expectedMessage), Arg.Any<CancellationToken>());
    }

    private ValueTask DidNotReceivePublishMessageAsync()
    {
        return this._twitchChatMessageChannel.DidNotReceive()
                   .PublishAsync(Arg.Any<TwitchChatMessage>(), Arg.Any<CancellationToken>());
    }

    private void ReceivedCurrentTime()
    {
        this._currentTimeSource.Received(1)
            .UtcNow();
    }

    private void DidNotReceiveCurrentTime()
    {
        this._currentTimeSource.DidNotReceive()
            .UtcNow();
    }

    [Fact]
    public async Task ThanksForBitsAsync()
    {
        await this._contributionThanks.ThankForBitsAsync(streamer: Streamer, user: GiftingUser, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{GiftingUser} for the bits");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForGiftingOneSubAsync()
    {
        await this._contributionThanks.ThankForGiftingSubAsync(streamer: Streamer, giftedBy: GiftingUser, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{GiftingUser} for gifting sub");

        this.ReceivedCurrentTime();
    }

    [Fact]
    public async Task ThankForGiftingMultipleSubsAsync()
    {
        await this._contributionThanks.ThankForMultipleGiftSubsAsync(streamer: Streamer, giftedBy: GiftingUser, count: 27, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{GiftingUser} for gifting subs");

        this.ReceivedCurrentTime();
    }

    [Fact]
    public async Task ThankForNewPaidSubAsync()
    {
        await this._contributionThanks.ThankForNewPaidSubAsync(streamer: Streamer, user: User, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{User} for subscribing");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForNewPrimeSubAsync()
    {
        await this._contributionThanks.ThankForNewPrimeSubAsync(streamer: Streamer, user: User, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{User} for subscribing");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForPaidReSubAsync()
    {
        await this._contributionThanks.ThankForPaidReSubAsync(streamer: Streamer, user: User, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{User} for resubscribing");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForPrimeReSubAsync()
    {
        await this._contributionThanks.ThankForPrimeReSubAsync(streamer: Streamer, user: User, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{User} for resubscribing");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForFollowAsync()
    {
        await this._contributionThanks.ThankForFollowAsync(streamer: Streamer, user: User, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }
}