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
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Actions.Services;

public sealed class ContributionThanksTests : TestBase
{
    private static readonly Viewer GiftingUser = Viewer.FromString(nameof(GiftingUser));
    private static readonly Viewer User = Viewer.FromString(nameof(User));

    private readonly IContributionThanks _contributionThanks;
    private readonly ICurrentTimeSource _currentTimeSource;
    private readonly ITwitchChannelState _twitchChannelState;
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    public ContributionThanksTests()
    {
        this._twitchChatMessageChannel = GetSubstitute<IMessageChannel<TwitchChatMessage>>();
        this._currentTimeSource = GetSubstitute<ICurrentTimeSource>();
        IOptions<TwitchBotOptions> options = Substitute.For<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions(
                                  channels: new()
                                            {
                                                new(MockReferenceData.Streamer.ToString(),
                                                    thanks: new(enabled: true),
                                                    raids: new(enabled: false, immediate: null, calmDown: null),
                                                    shoutOuts: new(enabled: false, friendChannels: null),
                                                    mileStones: new(enabled: false),
                                                    welcome: new(enabled: false))
                                            },
                                  heists: MockReferenceData.Heists,
                                  authentication: MockReferenceData.TwitchAuthentication,
                                  ignoredUsers: MockReferenceData.IgnoredUsers,
                                  milestones: MockReferenceData.TwitchMilestones));

        ITwitchChannelManager twitchChannelManager = GetSubstitute<ITwitchChannelManager>();
        this._twitchChannelState = GetSubstitute<ITwitchChannelState>();
        twitchChannelManager.GetStreamer(Arg.Any<Streamer>())
                            .Returns(this._twitchChannelState);

        this._contributionThanks = new ContributionThanks(twitchChannelManager: twitchChannelManager,
                                                          twitchChatMessageChannel: this._twitchChatMessageChannel,
                                                          currentTimeSource: this._currentTimeSource,
                                                          this.GetTypedLogger<ContributionThanks>());
    }

    private void MockThanksEnabled(bool enabled)
    {
        this._twitchChannelState.Settings.ThanksEnabled.Returns(enabled);
    }

    private ValueTask ReceivedPublishMessageAsync(string expectedMessage)
    {
        return this._twitchChatMessageChannel.Received(1)
                   .PublishAsync(Arg.Is<TwitchChatMessage>(t => t.Streamer == MockReferenceData.Streamer && t.Message == expectedMessage), Arg.Any<CancellationToken>());
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
        this.MockThanksEnabled(true);

        await this._contributionThanks.ThankForBitsAsync(streamer: MockReferenceData.Streamer, user: GiftingUser, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{GiftingUser} for the bits");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task NoThanksForBitsAsync()
    {
        this.MockThanksEnabled(false);

        await this._contributionThanks.ThankForBitsAsync(streamer: MockReferenceData.Streamer, user: GiftingUser, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForGiftingOneSubAsync()
    {
        this.MockThanksEnabled(true);

        await this._contributionThanks.ThankForGiftingSubAsync(streamer: MockReferenceData.Streamer, giftedBy: GiftingUser, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{GiftingUser} for gifting sub");

        this.ReceivedCurrentTime();
    }

    [Fact]
    public async Task NoThankForGiftingOneSubAsync()
    {
        this.MockThanksEnabled(false);

        await this._contributionThanks.ThankForGiftingSubAsync(streamer: MockReferenceData.Streamer, giftedBy: GiftingUser, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForGiftingMultipleSubsAsync()
    {
        this.MockThanksEnabled(true);

        await this._contributionThanks.ThankForMultipleGiftSubsAsync(streamer: MockReferenceData.Streamer, giftedBy: GiftingUser, count: 27, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{GiftingUser} for gifting subs");

        this.ReceivedCurrentTime();
    }

    [Fact]
    public async Task NoThanksForGiftingMultipleSubsAsync()
    {
        this.MockThanksEnabled(false);

        await this._contributionThanks.ThankForMultipleGiftSubsAsync(streamer: MockReferenceData.Streamer, giftedBy: GiftingUser, count: 27, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForNewPaidSubAsync()
    {
        this.MockThanksEnabled(true);

        await this._contributionThanks.ThankForNewPaidSubAsync(streamer: MockReferenceData.Streamer, user: User, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{User} for subscribing");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task NoThanksForNewPaidSubAsync()
    {
        this.MockThanksEnabled(false);

        await this._contributionThanks.ThankForNewPaidSubAsync(streamer: MockReferenceData.Streamer, user: User, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForNewPrimeSubAsync()
    {
        this.MockThanksEnabled(true);

        await this._contributionThanks.ThankForNewPrimeSubAsync(streamer: MockReferenceData.Streamer, user: User, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{User} for subscribing");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task NoThanksForNewPrimeSubAsync()
    {
        this.MockThanksEnabled(false);

        await this._contributionThanks.ThankForNewPrimeSubAsync(streamer: MockReferenceData.Streamer, user: User, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForPaidReSubAsync()
    {
        this.MockThanksEnabled(true);

        await this._contributionThanks.ThankForPaidReSubAsync(streamer: MockReferenceData.Streamer, user: User, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{User} for resubscribing");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task NoThanksForPaidReSubAsync()
    {
        this.MockThanksEnabled(false);

        await this._contributionThanks.ThankForPaidReSubAsync(streamer: MockReferenceData.Streamer, user: User, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForPrimeReSubAsync()
    {
        this.MockThanksEnabled(true);

        await this._contributionThanks.ThankForPrimeReSubAsync(streamer: MockReferenceData.Streamer, user: User, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{User} for resubscribing");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task NoThanksForPrimeReSubAsync()
    {
        this.MockThanksEnabled(false);

        await this._contributionThanks.ThankForPrimeReSubAsync(streamer: MockReferenceData.Streamer, user: User, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForFollowAsync()
    {
        await this._contributionThanks.ThankForFollowAsync(streamer: MockReferenceData.Streamer, user: User, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }
}