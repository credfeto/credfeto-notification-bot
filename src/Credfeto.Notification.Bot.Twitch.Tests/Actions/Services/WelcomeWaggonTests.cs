using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Actions.Services;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.StreamState;
using FunFair.Test.Common;
using Microsoft.Extensions.Options;
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

        IOptions<TwitchBotOptions> options = Substitute.For<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions { Channels = new() { new() { ChannelName = CHANNEL, Welcome = new() { Enabled = true } } } });

        this._welcomeWaggon = new WelcomeWaggon(options: options, twitchChatMessageChannel: this._twitchChatMessageChannel, this.GetTypedLogger<WelcomeWaggon>());
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