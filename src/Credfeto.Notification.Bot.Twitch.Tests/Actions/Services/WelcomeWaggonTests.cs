using System.Threading;
using System.Threading.Tasks;
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

public sealed class WelcomeWaggonTests : TestBase
{
    private readonly ITwitchChannelState _twitchChannelState;
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;
    private readonly ITwitchChatMessageGenerator _twitchChatMessageGenerator;
    private readonly IWelcomeWaggon _welcomeWaggon;

    public WelcomeWaggonTests()
    {
        this._twitchChatMessageChannel = GetSubstitute<IMessageChannel<TwitchChatMessage>>();

        ITwitchChannelManager twitchChannelManager = GetSubstitute<ITwitchChannelManager>();

        this._twitchChannelState = GetSubstitute<ITwitchChannelState>();
        twitchChannelManager.GetStreamer(MockReferenceData.Streamer)
                            .Returns(this._twitchChannelState);

        this._twitchChatMessageGenerator = GetSubstitute<ITwitchChatMessageGenerator>();
        this._twitchChatMessageGenerator.WelcomeMessage(Arg.Any<Viewer>())
            .Returns(x =>
                     {
                         Viewer user = x.Arg<Viewer>();

                         return $"Hi @{user}";
                     });

        this._welcomeWaggon = new WelcomeWaggon(twitchChannelManager: twitchChannelManager,
                                                twitchChatMessageGenerator: this._twitchChatMessageGenerator,
                                                twitchChatMessageChannel: this._twitchChatMessageChannel,
                                                logger: this.GetTypedLogger<WelcomeWaggon>());
    }

    [Fact]
    public async Task WelcomeAsync()
    {
        this._twitchChannelState.Settings.ChatWelcomesEnabled.Returns(true);

        await this._welcomeWaggon.IssueWelcomeAsync(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, cancellationToken: CancellationToken.None);

        this.ReceivedWelcomeMessage();
        await this.ReceivedPublishMessageAsync($"Hi @{MockReferenceData.Viewer}");
    }

    [Fact]
    public async Task NotWelcomeWhenWelcomesDisabledAsync()
    {
        this._twitchChannelState.Settings.ChatWelcomesEnabled.Returns(false);

        await this._welcomeWaggon.IssueWelcomeAsync(streamer: MockReferenceData.Streamer, user: MockReferenceData.Viewer, cancellationToken: CancellationToken.None);

        this.DidNotReceiveWelcomeMessage();
        await this.DidNotReceivePublishMessageAsync();
    }

    private void ReceivedWelcomeMessage()
    {
        this._twitchChatMessageGenerator.Received(1)
            .WelcomeMessage(MockReferenceData.Viewer);
    }

    private void DidNotReceiveWelcomeMessage()
    {
        this._twitchChatMessageGenerator.DidNotReceive()
            .WelcomeMessage(Arg.Any<Viewer>());
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
}