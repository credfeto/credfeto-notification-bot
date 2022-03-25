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

public sealed class WelcomeWaggonTests : TestBase
{
    private const string CHANNEL = nameof(CHANNEL);
    private const string USER = nameof(USER);
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;
    private readonly IWelcomeWaggon _welcomeWaggon;

    public WelcomeWaggonTests()
    {
        this._twitchChatMessageChannel = GetSubstitute<IMessageChannel<TwitchChatMessage>>();

        this._welcomeWaggon = new WelcomeWaggon(twitchChatMessageChannel: this._twitchChatMessageChannel, this.GetTypedLogger<WelcomeWaggon>());
    }

    [Fact]
    public async Task WelcomeAsync()
    {
        await this._welcomeWaggon.IssueWelcomeAsync(channel: CHANNEL, user: USER, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"Hi @{USER}");
    }

    private ValueTask ReceivedPublishMessageAsync(string expectedMessage)
    {
        return this._twitchChatMessageChannel.Received(1)
                   .PublishAsync(Arg.Is<TwitchChatMessage>(t => t.Channel == CHANNEL && t.Message == expectedMessage), Arg.Any<CancellationToken>());
    }
}