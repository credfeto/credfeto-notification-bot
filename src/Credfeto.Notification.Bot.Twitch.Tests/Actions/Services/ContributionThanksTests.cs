using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Actions.Services;
using Credfeto.Notification.Bot.Twitch.StreamState;
using FunFair.Test.Common;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Actions.Services;

public sealed class ContributionThanksTests : TestBase
{
    private const string CHANNEL = nameof(CHANNEL);
    private const string GIFTING_USER = nameof(GIFTING_USER);
    private const string USER = nameof(USER);

    private readonly IContributionThanks _contributionThanks;
    private readonly ICurrentTimeSource _currentTimeSource;
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    public ContributionThanksTests()
    {
        this._twitchChatMessageChannel = GetSubstitute<IMessageChannel<TwitchChatMessage>>();
        this._currentTimeSource = GetSubstitute<ICurrentTimeSource>();

        this._contributionThanks =
            new ContributionThanks(twitchChatMessageChannel: this._twitchChatMessageChannel, currentTimeSource: this._currentTimeSource, this.GetTypedLogger<ContributionThanks>());
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
        await this._contributionThanks.ThankForBitsAsync(channel: CHANNEL, user: GIFTING_USER, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{GIFTING_USER} for the bits");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForGiftingOneSubAsync()
    {
        await this._contributionThanks.ThankForGiftingSubAsync(channel: CHANNEL, giftedBy: GIFTING_USER, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{GIFTING_USER} for gifting sub");

        this.ReceivedCurrentTime();
    }

    [Fact]
    public async Task ThankForGiftingMultipleSubsAsync()
    {
        await this._contributionThanks.ThankForMultipleGiftSubsAsync(channel: CHANNEL, giftedBy: GIFTING_USER, count: 27, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{GIFTING_USER} for gifting subs");

        this.ReceivedCurrentTime();
    }

    [Fact]
    public async Task ThankForNewPaidSubAsync()
    {
        await this._contributionThanks.ThankForNewPaidSubAsync(channel: CHANNEL, user: USER, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{USER} for subscribing");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForNewPrimeSubAsync()
    {
        await this._contributionThanks.ThankForNewPrimeSubAsync(channel: CHANNEL, user: USER, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{USER} for subscribing");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForPaidReSubAsync()
    {
        await this._contributionThanks.ThankForPaidReSubAsync(channel: CHANNEL, user: USER, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{USER} for resubscribing");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForPrimeReSubAsync()
    {
        await this._contributionThanks.ThankForPrimeReSubAsync(channel: CHANNEL, user: USER, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Thanks @{USER} for resubscribing");

        this.DidNotReceiveCurrentTime();
    }

    [Fact]
    public async Task ThankForFollowAsync()
    {
        await this._contributionThanks.ThankForFollowAsync(channel: CHANNEL, user: USER, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();

        this.DidNotReceiveCurrentTime();
    }
}