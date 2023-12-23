using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using Mediator;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchWatchChannelFollowerNotificationHandlerTests : TestBase
{
    private static readonly Streamer ModdingForStreamer = MockReferenceData.Streamer.Next();
    private static readonly Streamer RandomStreamer = MockReferenceData.Streamer.Next();
    private readonly ITwitchFollowerDetector _followerDetector;
    private readonly INotificationHandler<TwitchWatchChannel> _notificationHandler;
    private readonly IOptions<TwitchBotOptions> _options;

    public TwitchWatchChannelFollowerNotificationHandlerTests()
    {
        this._followerDetector = GetSubstitute<ITwitchFollowerDetector>();

        this._options = GetSubstitute<IOptions<TwitchBotOptions>>();
        this._options.Value.Returns(new TwitchBotOptions(authentication: MockReferenceData.TwitchAuthentication,
                                                         milestones: MockReferenceData.TwitchMilestones,
                                                         ignoredUsers: MockReferenceData.IgnoredUsers,
                                                         chatCommands: Array.Empty<TwitchChatCommand>(),
                                                         channels:
                                                         [
                                                             new(channelName: ModdingForStreamer.Value,
                                                                 shoutOuts: MockReferenceData.TwitchChannelShoutout,
                                                                 raids: MockReferenceData.TwitchChannelRaids,
                                                                 thanks: MockReferenceData.TwitchChannelThanks,
                                                                 welcome: MockReferenceData.TwitchChannelWelcome,
                                                                 mileStones: MockReferenceData.TwitchChanelMileStone)
                                                         ]));

        this._notificationHandler = new TwitchWatchChannelFollowerNotificationHandler(options: this._options,
                                                                                      followerDetector: this._followerDetector,
                                                                                      this.GetTypedLogger<TwitchWatchChannelFollowerNotificationHandler>());
    }

    [Fact]
    public async Task ShouldEnableWatchingChannelAsync()
    {
        TwitchWatchChannel notification = new(new(Id: 42, ModdingForStreamer.ToViewer(), IsStreamer: false, DateCreated: DateTimeOffset.MinValue));
        await this._notificationHandler.Handle(notification: notification, cancellationToken: CancellationToken.None);

        await this._followerDetector.Received(1)
                  .EnableAsync(Arg.Any<TwitchUser>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ShouldNotEnableWatchingChannelAsync()
    {
        TwitchWatchChannel notification = new(new(Id: 42, RandomStreamer.ToViewer(), IsStreamer: false, DateCreated: DateTimeOffset.MinValue));
        await this._notificationHandler.Handle(notification: notification, cancellationToken: CancellationToken.None);

        await this._followerDetector.DidNotReceive()
                  .EnableAsync(Arg.Any<TwitchUser>(), Arg.Any<CancellationToken>());
    }
}