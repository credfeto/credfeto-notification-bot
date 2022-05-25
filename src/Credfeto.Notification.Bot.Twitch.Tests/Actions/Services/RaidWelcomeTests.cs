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

public sealed class RaidWelcomeTests : TestBase
{
    private static readonly Streamer Streamer = Streamer.FromString(nameof(Streamer));
    private static readonly Viewer Raider = Viewer.FromString(nameof(Raider));
    private readonly IRaidWelcome _raidWelcome;
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    public RaidWelcomeTests()
    {
        this._twitchChatMessageChannel = GetSubstitute<IMessageChannel<TwitchChatMessage>>();

        ITwitchChannelManager twitchChannelManager = GetSubstitute<ITwitchChannelManager>();
        ITwitchChannelState twitchChannelState = GetSubstitute<ITwitchChannelState>();
        twitchChannelManager.GetStreamer(Arg.Any<Streamer>())
                            .Returns(twitchChannelState);
        twitchChannelState.Settings.RaidWelcomesEnabled.Returns(true);

        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions
                              {
                                  Channels = new()
                                             {
                                                 new()
                                                 {
                                                     ChannelName = Streamer.Value,
                                                     Raids = new()
                                                             {
                                                                 Immediate = new[]
                                                                             {
                                                                                 "!raiders"
                                                                             },
                                                                 CalmDown = new[]
                                                                            {
                                                                                "!tag"
                                                                            }
                                                             }
                                                 }
                                             }
                              });

        this._raidWelcome = new RaidWelcome(options: options, twitchChannelManager: twitchChannelManager, twitchChatMessageChannel: this._twitchChatMessageChannel, this.GetTypedLogger<RaidWelcome>());
    }

    [Fact]
    public async Task IssueRaidWelcomeAsync()
    {
        await this._raidWelcome.IssueRaidWelcomeAsync(streamer: Streamer, raider: Raider, cancellationToken: CancellationToken.None);

        const string raidWelcome = @"
♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫
GlitchLit  GlitchLit  GlitchLit Welcome raiders! GlitchLit GlitchLit GlitchLit
♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫";

        await this.ReceivedPublishMessageAsync("!raiders");
        await this.ReceivedPublishMessageAsync(raidWelcome);
        await this.ReceivedPublishMessageAsync($"Thanks @{Raider} for the raid");
        await this.ReceivedPublishMessageAsync($"!so @{Raider}");
        await this.ReceivedPublishMessageAsync("!tag");
    }

    private ValueTask ReceivedPublishMessageAsync(string expectedMessage)
    {
        return this._twitchChatMessageChannel.Received(1)
                   .PublishAsync(Arg.Is<TwitchChatMessage>(t => t.Streamer == Streamer && t.Message == expectedMessage), Arg.Any<CancellationToken>());
    }
}