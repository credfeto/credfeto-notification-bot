using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using Credfeto.Notification.Bot.Twitch.StreamState;
using FunFair.Test.Common;
using MediatR;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchFollowerMilestoneReachedNotificationHandlerTests : TestBase
{
    private static readonly Streamer Streamer = Streamer.FromString(nameof(Streamer));

    private readonly INotificationHandler<TwitchFollowerMilestoneReached> _notificationHandler;
    private readonly ITwitchChannelManager _twitchChannelManager;
    private readonly ITwitchChannelState _twitchChannelState;
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    public TwitchFollowerMilestoneReachedNotificationHandlerTests()
    {
        this._twitchChannelManager = GetSubstitute<ITwitchChannelManager>();
        this._twitchChannelState = GetSubstitute<ITwitchChannelState>();

        this._twitchChannelManager.GetStreamer(Streamer)
            .Returns(this._twitchChannelState);

        this._twitchChatMessageChannel = GetSubstitute<IMessageChannel<TwitchChatMessage>>();

        this._notificationHandler = new TwitchFollowerMilestoneReachedNotificationHandler(twitchChannelManager: this._twitchChannelManager,
                                                                                          twitchChatMessageChannel: this._twitchChatMessageChannel,
                                                                                          this.GetTypedLogger<TwitchFollowerMilestoneReachedNotificationHandler>());
    }

    [Fact]
    public async Task WhenMilestonesEnabledShouldAnnounceAsync()
    {
        this._twitchChannelState.Settings.AnnounceMilestonesEnabled.Returns(true);

        TwitchFollowerMilestoneReached notification = new(streamer: Streamer, milestoneReached: 100, nextMilestone: 200, progress: 0);
        await this._notificationHandler.Handle(notification: notification, cancellationToken: CancellationToken.None);

        await this._twitchChatMessageChannel.Received(1)
                  .PublishAsync(Arg.Is<TwitchChatMessage>(t => t.Streamer == Streamer &&
                                                               t.Message == $"/me @{notification.Streamer} Woo! {notification.MilestoneReached} followers reached!"),
                                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task WhenMilestonesDisabledShouldAnnounceAsync()
    {
        this._twitchChannelState.Settings.AnnounceMilestonesEnabled.Returns(false);

        TwitchFollowerMilestoneReached notification = new(streamer: Streamer, milestoneReached: 100, nextMilestone: 200, progress: 0);
        await this._notificationHandler.Handle(notification: notification, cancellationToken: CancellationToken.None);

        await this._twitchChatMessageChannel.DidNotReceive()
                  .PublishAsync(Arg.Any<TwitchChatMessage>(), Arg.Any<CancellationToken>());
    }
}