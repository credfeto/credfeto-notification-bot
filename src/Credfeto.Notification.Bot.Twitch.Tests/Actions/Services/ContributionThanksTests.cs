using System.Threading;
using System.Threading.Tasks;
using Credfeto.Date.Interfaces;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Actions.Services;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.StreamState;
using FunFair.Test.Common;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Actions.Services;

public sealed class ContributionThanksTests : TestBase
{
    private const int BITS_GIVEN = 42;
    private static readonly Viewer GiftingUser = Viewer.FromString(nameof(GiftingUser));

    private readonly IContributionThanks _contributionThanks;
    private readonly ICurrentTimeSource _currentTimeSource;
    private readonly ITwitchChannelState _twitchChannelState;
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;
    private readonly ITwitchChatMessageGenerator _twitchChatMessageGenerator;

    public ContributionThanksTests()
    {
        this._twitchChatMessageChannel = GetSubstitute<IMessageChannel<TwitchChatMessage>>();
        this._currentTimeSource = GetSubstitute<ICurrentTimeSource>();

        ITwitchChannelManager twitchChannelManager = GetSubstitute<ITwitchChannelManager>();
        this._twitchChannelState = GetSubstitute<ITwitchChannelState>();
        twitchChannelManager.GetStreamer(Arg.Any<Streamer>())
                            .Returns(this._twitchChannelState);

        ITwitchChatMessageGenerator twitchChatMessageGenerator = BuildTwitchChatMessageGenerator();

        this._twitchChatMessageGenerator = twitchChatMessageGenerator;

        this._contributionThanks = new ContributionThanks(twitchChannelManager: twitchChannelManager,
                                                          twitchChatMessageChannel: this._twitchChatMessageChannel,
                                                          currentTimeSource: this._currentTimeSource,
                                                          twitchChatMessageGenerator: this._twitchChatMessageGenerator,
                                                          logger: this.GetTypedLogger<ContributionThanks>());
    }

    private static ITwitchChatMessageGenerator BuildTwitchChatMessageGenerator()
    {
        ITwitchChatMessageGenerator twitchChatMessageGenerator = GetSubstitute<ITwitchChatMessageGenerator>();
        twitchChatMessageGenerator.ThanksForBits(Arg.Any<Viewer>(), Arg.Any<int>())
                                  .Returns(x =>
                                           {
                                               Viewer user = x.Arg<Viewer>();

                                               return $"Thanks @{user} for the bits";
                                           });

        twitchChatMessageGenerator.ThanksForNewPaidSub(Arg.Any<Viewer>())
                                  .Returns(x =>
                                           {
                                               Viewer user = x.Arg<Viewer>();

                                               return $"Thanks @{user} for subscribing";
                                           });
        twitchChatMessageGenerator.ThanksForNewPrimeSub(Arg.Any<Viewer>())
                                  .Returns(x =>
                                           {
                                               Viewer user = x.Arg<Viewer>();

                                               return $"Thanks @{user} for subscribing";
                                           });
        twitchChatMessageGenerator.ThanksForPaidReSub(Arg.Any<Viewer>())
                                  .Returns(x =>
                                           {
                                               Viewer user = x.Arg<Viewer>();

                                               return $"Thanks @{user} for resubscribing";
                                           });
        twitchChatMessageGenerator.ThanksForPrimeReSub(Arg.Any<Viewer>())
                                  .Returns(x =>
                                           {
                                               Viewer user = x.Arg<Viewer>();

                                               return $"Thanks @{user} for resubscribing";
                                           });
        twitchChatMessageGenerator.ThanksForGiftingMultipleSubs(Arg.Any<Viewer>())
                                  .Returns(x =>
                                           {
                                               Viewer giftedBy = x.Arg<Viewer>();

                                               return $"Thanks @{giftedBy} for gifting subs";
                                           });

        twitchChatMessageGenerator.ThanksForGiftingOneSub(Arg.Any<Viewer>())
                                  .Returns(x =>
                                           {
                                               Viewer giftedBy = x.Arg<Viewer>();

                                               return $"Thanks @{giftedBy} for gifting sub";
                                           });

        return twitchChatMessageGenerator;
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

        await this._contributionThanks.ThankForBitsAsync(streamer: MockReferenceData.Streamer, user: GiftingUser, bitsGiven: BITS_GIVEN, cancellationToken: CancellationToken.None);

        this.ReceivedThanksForBits();

        await this.ReceivedPublishMessageAsync($"Thanks @{GiftingUser} for the bits");

        this.DidNotReceiveCurrentTime();
    }

    private void ReceivedThanksForBits()
    {
        this._twitchChatMessageGenerator.Received(1)
            .ThanksForBits(giftedBy: GiftingUser, bitsGiven: BITS_GIVEN);
    }

    private void DidNotReceiveThanksForBits()
    {
        this._twitchChatMessageGenerator.DidNotReceive()
            .ThanksForBits(Arg.Any<Viewer>(), Arg.Any<int>());
    }

    private void ReceivedThanksForNewPaidSub()
    {
        this._twitchChatMessageGenerator.Received(1)
            .ThanksForNewPaidSub(MockReferenceData.Viewer);
    }

    private void DidNotReceiveThanksForNewPaidSub()
    {
        this._twitchChatMessageGenerator.DidNotReceive()
            .ThanksForNewPaidSub(Arg.Any<Viewer>());
    }

    private void ReceivedThanksForPaidReSub()
    {
        this._twitchChatMessageGenerator.Received(1)
            .ThanksForPaidReSub(MockReferenceData.Viewer);
    }

    private void DidNotReceiveThanksForPaidReSub()
    {
        this._twitchChatMessageGenerator.DidNotReceive()
            .ThanksForPaidReSub(Arg.Any<Viewer>());
    }

    private void ReceivedThanksForNewPrimeSub()
    {
        this._twitchChatMessageGenerator.Received(1)
            .ThanksForNewPrimeSub(MockReferenceData.Viewer);
    }

    private void DidNotReceiveThanksForNewPrimeSub()
    {
        this._twitchChatMessageGenerator.DidNotReceive()
            .ThanksForNewPrimeSub(Arg.Any<Viewer>());
    }

    private void ReceivedThanksForPrimeReSub()
    {
        this._twitchChatMessageGenerator.Received(1)
            .ThanksForPrimeReSub(MockReferenceData.Viewer);
    }

    private void DidNotReceiveThanksForPrimeReSub()
    {
        this._twitchChatMessageGenerator.DidNotReceive()
            .ThanksForPrimeReSub(Arg.Any<Viewer>());
    }

    private void ReceivedThanksForGiftingMultipleSubs()
    {
        this._twitchChatMessageGenerator.Received(1)
            .ThanksForGiftingMultipleSubs(GiftingUser);
    }

    private void DidNotReceiveThanksForGiftingMultipleSubs()
    {
        this._twitchChatMessageGenerator.DidNotReceive()
            .ThanksForGiftingMultipleSubs(Arg.Any<Viewer>());
    }

    private void ReceivedThanksForGiftingOneSub()
    {
        this._twitchChatMessageGenerator.Received(1)
            .ThanksForGiftingOneSub(GiftingUser);
    }

    private void DidNotReceiveThanksForGiftingOneSub()
    {
        this._twitchChatMessageGenerator.DidNotReceive()
            .ThanksForGiftingOneSub(Arg.Any<Viewer>());
    }

    [Fact]
    public async Task NoThanksForBitsAsync()
    {
        this.MockThanksEnabled(false);

        await this._contributionThanks.ThankForBitsAsync(streamer: MockReferenceData.Streamer, user: GiftingUser, bitsGiven: BITS_GIVEN, cancellationToken: CancellationToken.None);

        this.DidNotReceiveThanksForBits();
        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForGiftingOneSubAsync()
    {
        this.MockThanksEnabled(true);

        await this._contributionThanks.ThankForGiftingSubAsync(streamer: MockReferenceData.Streamer, giftedBy: GiftingUser, cancellationToken: CancellationToken.None);

        this.ReceivedThanksForGiftingOneSub();
        await this.ReceivedPublishMessageAsync($"Thanks @{GiftingUser} for gifting sub");

        this.ReceivedCurrentTime();
    }

    [Fact]
    public async Task NoThankForGiftingOneSubAsync()
    {
        this.MockThanksEnabled(false);

        await this._contributionThanks.ThankForGiftingSubAsync(streamer: MockReferenceData.Streamer, giftedBy: GiftingUser, cancellationToken: CancellationToken.None);

        this.DidNotReceiveThanksForGiftingOneSub();
        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForGiftingMultipleSubsAsync()
    {
        this.MockThanksEnabled(true);

        await this._contributionThanks.ThankForMultipleGiftSubsAsync(streamer: MockReferenceData.Streamer, giftedBy: GiftingUser, count: 27, cancellationToken: CancellationToken.None);

        this.ReceivedThanksForGiftingMultipleSubs();
        await this.ReceivedPublishMessageAsync($"Thanks @{GiftingUser} for gifting subs");

        this.ReceivedCurrentTime();
    }

    [Fact]
    public async Task NoThanksForGiftingMultipleSubsAsync()
    {
        this.MockThanksEnabled(false);

        await this._contributionThanks.ThankForMultipleGiftSubsAsync(streamer: MockReferenceData.Streamer, giftedBy: GiftingUser, count: 27, cancellationToken: CancellationToken.None);

        this.DidNotReceiveThanksForGiftingMultipleSubs();
        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForNewPaidSubAsync()
    {
        this.MockThanksEnabled(true);

        await this._contributionThanks.ThankForNewPaidSubAsync(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, cancellationToken: CancellationToken.None);

        this.ReceivedThanksForNewPaidSub();
        await this.ReceivedPublishMessageAsync($"Thanks @{MockReferenceData.Viewer} for subscribing");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task NoThanksForNewPaidSubAsync()
    {
        this.MockThanksEnabled(false);

        await this._contributionThanks.ThankForNewPaidSubAsync(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, cancellationToken: CancellationToken.None);

        this.DidNotReceiveThanksForNewPaidSub();
        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForNewPrimeSubAsync()
    {
        this.MockThanksEnabled(true);

        await this._contributionThanks.ThankForNewPrimeSubAsync(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, cancellationToken: CancellationToken.None);

        this.ReceivedThanksForNewPrimeSub();
        await this.ReceivedPublishMessageAsync($"Thanks @{MockReferenceData.Viewer} for subscribing");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task NoThanksForNewPrimeSubAsync()
    {
        this.MockThanksEnabled(false);

        await this._contributionThanks.ThankForNewPrimeSubAsync(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, cancellationToken: CancellationToken.None);

        this.DidNotReceiveThanksForNewPrimeSub();
        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForPaidReSubAsync()
    {
        this.MockThanksEnabled(true);

        await this._contributionThanks.ThankForPaidReSubAsync(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, cancellationToken: CancellationToken.None);

        this.ReceivedThanksForPaidReSub();
        await this.ReceivedPublishMessageAsync($"Thanks @{MockReferenceData.Viewer} for resubscribing");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task NoThanksForPaidReSubAsync()
    {
        this.MockThanksEnabled(false);

        await this._contributionThanks.ThankForPaidReSubAsync(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, cancellationToken: CancellationToken.None);

        this.DidNotReceiveThanksForPaidReSub();
        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForPrimeReSubAsync()
    {
        this.MockThanksEnabled(true);

        await this._contributionThanks.ThankForPrimeReSubAsync(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, cancellationToken: CancellationToken.None);

        this.ReceivedThanksForPrimeReSub();
        await this.ReceivedPublishMessageAsync($"Thanks @{MockReferenceData.Viewer} for resubscribing");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task NoThanksForPrimeReSubAsync()
    {
        this.MockThanksEnabled(false);

        await this._contributionThanks.ThankForPrimeReSubAsync(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, cancellationToken: CancellationToken.None);

        this.DidNotReceiveThanksForPrimeReSub();
        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForFollowAsync()
    {
        await this._contributionThanks.ThankForFollowAsync(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }
}