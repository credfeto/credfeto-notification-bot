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

public sealed class RaidWelcomeTests : TestBase
{
    private const string CHANNEL = nameof(CHANNEL);
    private const string RAIDER = nameof(RAIDER);
    private readonly IRaidWelcome _raidWelcome;
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    public RaidWelcomeTests()
    {
        this._twitchChatMessageChannel = GetSubstitute<IMessageChannel<TwitchChatMessage>>();
        IOptions<TwitchBotOptions> options = Substitute.For<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions { Channels = new() { new() { ChannelName = CHANNEL, Raids = new() { Enabled = true } } } });

        this._raidWelcome = new RaidWelcome(options: options, twitchChatMessageChannel: this._twitchChatMessageChannel, this.GetTypedLogger<RaidWelcome>());
    }

    [Fact]
    public async Task IssueRaidWelcomeAsync()
    {
        await this._raidWelcome.IssueRaidWelcomeAsync(channel: CHANNEL, raider: RAIDER, cancellationToken: CancellationToken.None);

        const string raidWelcome = @"
♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫
GlitchLit  GlitchLit  GlitchLit Welcome raiders! GlitchLit GlitchLit GlitchLit
♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫";

        await this.ReceivedPublishMessageAsync(raidWelcome);
        await this.ReceivedPublishMessageAsync($"Thanks @{RAIDER} for the raid");
        await this.ReceivedPublishMessageAsync($"!so @{RAIDER}");
    }

    private ValueTask ReceivedPublishMessageAsync(string expectedMessage)
    {
        return this._twitchChatMessageChannel.Received(1)
                   .PublishAsync(Arg.Is<TwitchChatMessage>(t => t.Channel == CHANNEL && t.Message == expectedMessage), Arg.Any<CancellationToken>());
    }
}