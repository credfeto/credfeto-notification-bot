using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using MediatR;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Services;

public sealed class TwitchChannelManagerTests : TestBase
{
    private static readonly Streamer Streamer = Streamer.FromString(nameof(Streamer));
    private static readonly Streamer Guest1 = Streamer.FromString(nameof(Guest1));
    private static readonly Streamer Guest2 = Streamer.FromString(nameof(Guest2));

    private readonly IMediator _mediator;
    private readonly ITwitchChannelManager _twitchChannelManager;
    private readonly ITwitchStreamDataManager _twitchStreamerDataManager;
    private readonly IUserInfoService _userInfoService;

    public TwitchChannelManagerTests()
    {
        this._userInfoService = GetSubstitute<IUserInfoService>();
        this._twitchStreamerDataManager = GetSubstitute<ITwitchStreamDataManager>();
        this._mediator = GetSubstitute<IMediator>();

        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions
                              {
                                  Authentication = new() { ClientId = "Invalid", ClientSecret = "Invalid", ClientAccessToken = "Invalid" },
                                  Channels = new()
                                             {
                                                 new()
                                                 {
                                                     ChannelName = Streamer.Value,
                                                     Welcome = new() { Enabled = true },
                                                     MileStones = new() { Enabled = true },
                                                     Raids = new() { Enabled = true },
                                                     Thanks = new() { Enabled = true },
                                                     ShoutOuts = new()
                                                                 {
                                                                     Enabled = true,
                                                                     FriendChannels = new()
                                                                                      {
                                                                                          new() { Channel = Guest1.Value, Message = "!guest" }, new() { Channel = Guest2.Value, Message = null }
                                                                                      }
                                                                 }
                                                 }
                                             }
                              });

        this._twitchChannelManager = new TwitchChannelManager(options: options,
                                                              userInfoService: this._userInfoService,
                                                              twitchStreamDataManager: this._twitchStreamerDataManager,
                                                              mediator: this._mediator,
                                                              this.GetTypedLogger<TwitchChannelManager>());
    }

    [Fact]
    public async Task StreamOnlineOfflineAsync()
    {
        ITwitchChannelState twitchChannelState = this._twitchChannelManager.GetStreamer(Streamer);

        await twitchChannelState.OnlineAsync(gameName: "FunGame", new(year: 2020, month: 1, day: 1));

        Assert.True(condition: twitchChannelState.IsOnline, userMessage: "Should be online");

        await this._twitchStreamerDataManager.Received(1)
                  .RecordStreamStartAsync(streamer: Streamer, new(year: 2020, month: 1, day: 1));

        twitchChannelState.Offline();

        Assert.False(condition: twitchChannelState.IsOnline, userMessage: "Should be offline");
    }
}