using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Actions.Services;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Services;
using Credfeto.Notification.Bot.Twitch.StreamState;
using FunFair.Test.Common;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Actions.Services;

public sealed class ShoutoutJoinerTests : TestBase
{
    private static readonly Channel ChannelShoutOutsEnabled = Types.ChannelFromString(nameof(ChannelShoutOutsEnabled));
    private static readonly Channel ChannelShoutOutsDisabled = Types.ChannelFromString(nameof(ChannelShoutOutsDisabled));
    private static readonly User VisitingChannelFriendWithMessage = Types.UserFromString(nameof(VisitingChannelFriendWithMessage));
    private static readonly User VisitingChannelFriendWithNoMessage = Types.UserFromString(nameof(VisitingChannelFriendWithNoMessage));
    private static readonly User VisitingChannel = Types.UserFromString(nameof(VisitingChannel));
    private readonly IShoutoutJoiner _shoutoutJoiner;
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    public ShoutoutJoinerTests()
    {
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions
                              {
                                  Channels = new()
                                             {
                                                 new()
                                                 {
                                                     ChannelName = ChannelShoutOutsEnabled.ToString(),
                                                     ShoutOuts = new()
                                                                 {
                                                                     Enabled = true,
                                                                     FriendChannels = new()
                                                                                      {
                                                                                          new()
                                                                                          {
                                                                                              Channel = VisitingChannelFriendWithMessage.ToString(),
                                                                                              Message = "Check out this weird and wonderful streamer!"
                                                                                          },
                                                                                          new() { Channel = VisitingChannelFriendWithNoMessage.ToString(), Message = null }
                                                                                      }
                                                                 }
                                                 }
                                             }
                              });

        this._twitchChatMessageChannel = GetSubstitute<IMessageChannel<TwitchChatMessage>>();

        this._shoutoutJoiner = new ShoutoutJoiner(options: options, twitchChatMessageChannel: this._twitchChatMessageChannel, this.GetTypedLogger<ShoutoutJoiner>());
    }

    private ValueTask ReceivedPublishMessageAsync(string expectedMessage)
    {
        return this._twitchChatMessageChannel.Received(1)
                   .PublishAsync(Arg.Is<TwitchChatMessage>(t => t.Channel == ChannelShoutOutsEnabled && t.Message == expectedMessage), Arg.Any<CancellationToken>());
    }

    private ValueTask DidNotReceivePublishMessageAsync()
    {
        return this._twitchChatMessageChannel.DidNotReceive()
                   .PublishAsync(Arg.Any<TwitchChatMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task IssueShoutoutUnknownStreamerAsync()
    {
        TwitchUser visitingStreamer = new(id: "123456", userName: VisitingChannel, isStreamer: true, new(year: 2020, month: 1, day: 1));

        await this._shoutoutJoiner.IssueShoutoutAsync(channel: ChannelShoutOutsEnabled, visitingStreamer: visitingStreamer, isRegular: false, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();
    }

    [Fact]
    public async Task IssueShoutoutDisabledAsync()
    {
        TwitchUser visitingStreamer = new(id: "123456", userName: VisitingChannel, isStreamer: true, new(year: 2020, month: 1, day: 1));

        await this._shoutoutJoiner.IssueShoutoutAsync(channel: ChannelShoutOutsDisabled, visitingStreamer: visitingStreamer, isRegular: true, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();
    }

    [Fact]
    public async Task IssueShoutoutRegularStreamerVisitorAsync()
    {
        TwitchUser visitingStreamer = new(id: "123456", userName: VisitingChannel, isStreamer: true, new(year: 2020, month: 1, day: 1));

        await this._shoutoutJoiner.IssueShoutoutAsync(channel: ChannelShoutOutsEnabled, visitingStreamer: visitingStreamer, isRegular: true, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"!so @{VisitingChannel}");
    }

    [Fact]
    public async Task IssueShoutoutFriendStreamerVisitorWithNoCustomMessageAsync()
    {
        TwitchUser visitingStreamer = new(id: "123456", userName: VisitingChannelFriendWithNoMessage, isStreamer: true, new(year: 2020, month: 1, day: 1));

        await this._shoutoutJoiner.IssueShoutoutAsync(channel: ChannelShoutOutsEnabled, visitingStreamer: visitingStreamer, isRegular: true, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"!so @{VisitingChannelFriendWithNoMessage}");
    }

    [Fact]
    public async Task IssueShoutoutFriendStreamerVisitorWithCustomMessageAsync()
    {
        TwitchUser visitingStreamer = new(id: "123456", userName: VisitingChannelFriendWithMessage, isStreamer: true, new(year: 2020, month: 1, day: 1));

        await this._shoutoutJoiner.IssueShoutoutAsync(channel: ChannelShoutOutsEnabled, visitingStreamer: visitingStreamer, isRegular: true, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync("Check out this weird and wonderful streamer!");
    }
}