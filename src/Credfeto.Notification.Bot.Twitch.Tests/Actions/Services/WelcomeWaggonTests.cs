using System.Threading;
using System.Threading.Tasks;
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
    private static readonly Streamer Streamer = Streamer.FromString(nameof(Streamer));
    private static readonly Viewer User = Viewer.FromString(nameof(User));
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;
    private readonly IWelcomeWaggon _welcomeWaggon;

    public WelcomeWaggonTests()
    {
        this._twitchChatMessageChannel = GetSubstitute<IMessageChannel<TwitchChatMessage>>();

        ITwitchChannelManager twitchChannelManager = GetSubstitute<ITwitchChannelManager>();

        ITwitchChannelState channel = GetSubstitute<ITwitchChannelState>();
        channel.Settings.ChatWelcomesEnabled.Returns(true);
        twitchChannelManager.GetStreamer(Streamer)
                            .Returns(channel);

        this._welcomeWaggon = new WelcomeWaggon(twitchChannelManager: twitchChannelManager, twitchChatMessageChannel: this._twitchChatMessageChannel, this.GetTypedLogger<WelcomeWaggon>());
    }

    [Fact]
    public async Task WelcomeAsync()
    {
        await this._welcomeWaggon.IssueWelcomeAsync(streamer: Streamer, user: User, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Hi @{User}");
    }

    private ValueTask ReceivedPublishMessageAsync(string expectedMessage)
    {
        return this._twitchChatMessageChannel.Received(1)
                   .PublishAsync(Arg.Is<TwitchChatMessage>(t => t.Streamer == Streamer && t.Message == expectedMessage), Arg.Any<CancellationToken>());
    }
}