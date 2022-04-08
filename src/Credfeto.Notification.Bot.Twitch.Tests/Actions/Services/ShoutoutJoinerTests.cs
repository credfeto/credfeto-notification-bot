using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Actions.Services;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.StreamState;
using FunFair.Test.Common;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Actions.Services;

public sealed class ShoutoutJoinerTests : TestBase
{
    private const string CHANNEL_SHOUT_OUTS_ENABLED = nameof(CHANNEL_SHOUT_OUTS_ENABLED);
    private const string CHANNEL_SHOUT_OUTS_DISABLED = nameof(CHANNEL_SHOUT_OUTS_DISABLED);
    private const string VISITING_CHANNEL_FRIEND_WITH_MESSAGE = nameof(VISITING_CHANNEL_FRIEND_WITH_MESSAGE);
    private const string VISITING_CHANNEL_FRIEND_WITH_NO_MESSAGE = nameof(VISITING_CHANNEL_FRIEND_WITH_NO_MESSAGE);
    private const string VISITING_CHANNEL = nameof(VISITING_CHANNEL);
    private readonly IShoutoutJoiner _shoutoutJoiner;
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;
    private readonly ITwitchStreamDataManager _twitchStreamDataManager;

    public ShoutoutJoinerTests()
    {
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions
                              {
                                  Channels = new()
                                             {
                                                 new()
                                                 {
                                                     ChannelName = CHANNEL_SHOUT_OUTS_ENABLED,
                                                     ShoutOuts = new()
                                                                 {
                                                                     Enabled = true,
                                                                     FriendChannels = new()
                                                                                      {
                                                                                          new()
                                                                                          {
                                                                                              Channel = VISITING_CHANNEL_FRIEND_WITH_MESSAGE, Message = "Check out this weird and wonderful streamer!"
                                                                                          },
                                                                                          new() { Channel = VISITING_CHANNEL_FRIEND_WITH_NO_MESSAGE, Message = null }
                                                                                      }
                                                                 }
                                                 }
                                             }
                              });

        this._twitchChatMessageChannel = GetSubstitute<IMessageChannel<TwitchChatMessage>>();
        this._twitchStreamDataManager = GetSubstitute<ITwitchStreamDataManager>();

        this._shoutoutJoiner = new ShoutoutJoiner(options: options,
                                                  twitchChatMessageChannel: this._twitchChatMessageChannel,
                                                  twitchStreamDataManager: this._twitchStreamDataManager,
                                                  this.GetTypedLogger<ShoutoutJoiner>());
    }

    private ValueTask ReceivedPublishMessageAsync(string expectedMessage)
    {
        return this._twitchChatMessageChannel.Received(1)
                   .PublishAsync(Arg.Is<TwitchChatMessage>(t => t.Channel == CHANNEL_SHOUT_OUTS_ENABLED && t.Message == expectedMessage), Arg.Any<CancellationToken>());
    }

    private ValueTask DidNotReceivePublishMessageAsync()
    {
        return this._twitchChatMessageChannel.DidNotReceive()
                   .PublishAsync(Arg.Any<TwitchChatMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task IssueShoutoutUnknownStreamerAsync()
    {
        TwitchUser visitingStreamer = new(id: "123456", userName: VISITING_CHANNEL, isStreamer: true, new(year: 2020, month: 1, day: 1));

        await this._shoutoutJoiner.IssueShoutoutAsync(channel: CHANNEL_SHOUT_OUTS_ENABLED, visitingStreamer: visitingStreamer, isRegular: false, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();
    }

    [Fact]
    public async Task IssueShoutoutDisabledAsync()
    {
        TwitchUser visitingStreamer = new(id: "123456", userName: VISITING_CHANNEL, isStreamer: true, new(year: 2020, month: 1, day: 1));

        await this._shoutoutJoiner.IssueShoutoutAsync(channel: CHANNEL_SHOUT_OUTS_DISABLED, visitingStreamer: visitingStreamer, isRegular: true, cancellationToken: CancellationToken.None);

        await this.DidNotReceivePublishMessageAsync();
    }

    [Fact]
    public async Task IssueShoutoutRegularStreamerVisitorAsync()
    {
        TwitchUser visitingStreamer = new(id: "123456", userName: VISITING_CHANNEL, isStreamer: true, new(year: 2020, month: 1, day: 1));

        await this._shoutoutJoiner.IssueShoutoutAsync(channel: CHANNEL_SHOUT_OUTS_ENABLED, visitingStreamer: visitingStreamer, isRegular: true, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"!so @{VISITING_CHANNEL}");
    }

    [Fact]
    public async Task IssueShoutoutFriendStreamerVisitorWithNoCustomMessageAsync()
    {
        TwitchUser visitingStreamer = new(id: "123456", userName: VISITING_CHANNEL_FRIEND_WITH_NO_MESSAGE, isStreamer: true, new(year: 2020, month: 1, day: 1));

        await this._shoutoutJoiner.IssueShoutoutAsync(channel: CHANNEL_SHOUT_OUTS_ENABLED, visitingStreamer: visitingStreamer, isRegular: true, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync($"!so @{VISITING_CHANNEL_FRIEND_WITH_NO_MESSAGE}");
    }

    [Fact]
    public async Task IssueShoutoutFriendStreamerVisitorWithCustomMessageAsync()
    {
        TwitchUser visitingStreamer = new(id: "123456", userName: VISITING_CHANNEL_FRIEND_WITH_MESSAGE, isStreamer: true, new(year: 2020, month: 1, day: 1));

        await this._shoutoutJoiner.IssueShoutoutAsync(channel: CHANNEL_SHOUT_OUTS_ENABLED, visitingStreamer: visitingStreamer, isRegular: true, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync("Check out this weird and wonderful streamer!");
    }
}