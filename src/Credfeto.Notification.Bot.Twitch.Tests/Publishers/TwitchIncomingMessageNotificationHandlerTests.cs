using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using MediatR;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchIncomingMessageNotificationHandlerTests : TestBase
{
    private readonly INotificationHandler<TwitchIncomingMessage> _notificationHandler;
    private readonly ITwitchChannelManager _twitchChannelManager;
    private readonly ITwitchCustomMessageHandler _twitchCustomMessageHandler;

    public TwitchIncomingMessageNotificationHandlerTests()
    {
        this._twitchChannelManager = GetSubstitute<ITwitchChannelManager>();
        this._twitchCustomMessageHandler = GetSubstitute<ITwitchCustomMessageHandler>();
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions(authentication: MockReferenceData.TwitchAuthentication,
                                                   new()
                                                   {
                                                       new(channelName: ((Streamer)MockReferenceData.Streamer).Value,
                                                           raids: MockReferenceData.TwitchChannelRaids,
                                                           shoutOuts: MockReferenceData.TwitchChannelShoutout,
                                                           thanks: MockReferenceData.TwitchChannelThanks,
                                                           mileStones: MockReferenceData.TwitchChanelMileStone,
                                                           welcome: MockReferenceData.TwitchChannelWelcome)
                                                   },
                                                   heists: MockReferenceData.Heists,
                                                   marbles: null,
                                                   ignoredUsers: MockReferenceData.IgnoredUsers,
                                                   milestones: MockReferenceData.TwitchMilestones));

        this._notificationHandler = new TwitchIncomingMessageNotificationHandler(options: options,
                                                                                 twitchChannelManager: this._twitchChannelManager,
                                                                                 twitchCustomMessageHandler: this._twitchCustomMessageHandler,
                                                                                 this.GetTypedLogger<TwitchIncomingMessageNotificationHandler>());
    }

    [Fact]
    public async Task JdfiAsync()
    {
        // TODO: Implements some decent tests
        await this._notificationHandler.Handle(new(Streamer: MockReferenceData.Streamer, Chatter: MockReferenceData.Viewer, Message: "Banana"), cancellationToken: CancellationToken.None);

        this._twitchChannelManager.DidNotReceive()
            .GetStreamer(MockReferenceData.Streamer);
    }
}