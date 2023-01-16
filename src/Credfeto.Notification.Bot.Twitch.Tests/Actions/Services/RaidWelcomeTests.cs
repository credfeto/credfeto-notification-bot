using System;
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
using Xunit.Abstractions;

namespace Credfeto.Notification.Bot.Twitch.Tests.Actions.Services;

public sealed class RaidWelcomeTests : LoggingTestBase
{
    private const string IMMEDIATE_MSG = "!raiders";
    private const string CALM_DOWN_MSG = "!tag";
    private static readonly Viewer Raider = Viewer.FromString(nameof(Raider));
    private readonly IRaidWelcome _raidWelcome;
    private readonly ITwitchChannelState _twitchChannelState;
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    public RaidWelcomeTests(ITestOutputHelper output)
        : base(output)
    {
        this._twitchChatMessageChannel = GetSubstitute<IMessageChannel<TwitchChatMessage>>();

        ITwitchChannelManager twitchChannelManager = GetSubstitute<ITwitchChannelManager>();

        this._twitchChannelState = GetSubstitute<ITwitchChannelState>();
        twitchChannelManager.GetStreamer(Arg.Any<Streamer>())
                            .Returns(this._twitchChannelState);

        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions(authentication: MockReferenceData.TwitchAuthentication,
                                                   milestones: MockReferenceData.TwitchMilestones,
                                                   ignoredUsers: MockReferenceData.IgnoredUsers,
                                                   heists: MockReferenceData.Heists,
                                                   marbles: Array.Empty<TwitchChatTriggeredMessage>(),
                                                   channels: new TwitchModChannel[]
                                                             {
                                                                 new(channelName: ((Streamer)MockReferenceData.Streamer).Value,
                                                                     raids: new(enabled: false,
                                                                                new[]
                                                                                {
                                                                                    IMMEDIATE_MSG
                                                                                },
                                                                                new[]
                                                                                {
                                                                                    CALM_DOWN_MSG
                                                                                }),
                                                                     shoutOuts: MockReferenceData.TwitchChannelShoutout,
                                                                     thanks: MockReferenceData.TwitchChannelThanks,
                                                                     mileStones: MockReferenceData.TwitchChanelMileStone,
                                                                     welcome: MockReferenceData.TwitchChannelWelcome)
                                                             }));

        this._raidWelcome = new RaidWelcome(options: options, twitchChannelManager: twitchChannelManager, twitchChatMessageChannel: this._twitchChatMessageChannel, this.GetTypedLogger<RaidWelcome>());
    }

    [Fact]
    public async Task IssueRaidWelcomeAsync()
    {
        this._twitchChannelState.Settings.RaidWelcomesEnabled.Returns(true);

        await this._raidWelcome.IssueRaidWelcomeAsync(streamer: MockReferenceData.Streamer, raider: Raider, cancellationToken: CancellationToken.None);

        string raidWelcome = @"♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫" + Environment.NewLine + @"GlitchLit  GlitchLit  GlitchLit Welcome raiders! GlitchLit GlitchLit GlitchLit" + Environment.NewLine +
                             @"♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫";

        await this.ReceivedPublishMessageAsync(IMMEDIATE_MSG);
        await this.ReceivedPublishMessageAsync(raidWelcome);
        await this.ReceivedPublishMessageAsync($"Thanks @{Raider} for the raid");
        await this.ReceivedPublishMessageAsync($"!so @{Raider}");
        await this.ReceivedPublishMessageAsync($"/shoutout @{Raider}");
        await this.ReceivedPublishMessageAsync(CALM_DOWN_MSG);
    }

    [Fact]
    public async Task IssueRaidWelcomeNonModChannelAsync()
    {
        this._twitchChannelState.Settings.RaidWelcomesEnabled.Returns(false);

        await this._raidWelcome.IssueRaidWelcomeAsync(streamer: MockReferenceData.Streamer, raider: Raider, cancellationToken: CancellationToken.None);

        await this._twitchChatMessageChannel.DidNotReceive()
                  .PublishAsync(Arg.Any<TwitchChatMessage>(), Arg.Any<CancellationToken>());
    }

    private ValueTask ReceivedPublishMessageAsync(string expectedMessage)
    {
        return this._twitchChatMessageChannel.Received(1)
                   .PublishAsync(Arg.Is<TwitchChatMessage>(t => t.Streamer == MockReferenceData.Streamer && StringComparer.InvariantCultureIgnoreCase.Equals(t.Message.Trim(), expectedMessage)),
                                 Arg.Any<CancellationToken>());
    }
}