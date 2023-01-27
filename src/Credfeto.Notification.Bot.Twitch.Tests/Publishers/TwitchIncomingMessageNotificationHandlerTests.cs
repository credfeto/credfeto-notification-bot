using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Publishers;
using FunFair.Test.Common;
using Mediator;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Publishers;

public sealed class TwitchIncomingMessageNotificationHandlerTests : TestBase
{
    private readonly INotificationHandler<TwitchIncomingMessage> _notificationHandler;
    private readonly ITwitchChannelManager _twitchChannelManager;
    private readonly ITwitchChannelState _twitchChannelState;
    private readonly ITwitchCustomMessageHandler _twitchCustomMessageHandler;

    public TwitchIncomingMessageNotificationHandlerTests()
    {
        this._twitchChannelState = GetSubstitute<ITwitchChannelState>();
        this._twitchChannelManager = GetSubstitute<ITwitchChannelManager>();
        this._twitchChannelManager.GetStreamer(MockReferenceData.Streamer)
            .Returns(this._twitchChannelState);
        this._twitchCustomMessageHandler = GetSubstitute<ITwitchCustomMessageHandler>();
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(new TwitchBotOptions(authentication: MockReferenceData.TwitchAuthentication,
                                                   new TwitchModChannel[]
                                                   {
                                                       new(channelName: ((Streamer)MockReferenceData.Streamer).Value,
                                                           raids: MockReferenceData.TwitchChannelRaids,
                                                           shoutOuts: MockReferenceData.TwitchChannelShoutout,
                                                           thanks: MockReferenceData.TwitchChannelThanks,
                                                           mileStones: MockReferenceData.TwitchChanelMileStone,
                                                           welcome: MockReferenceData.TwitchChannelWelcome)
                                                   },
                                                   chatCommands: Array.Empty<TwitchChatCommand>(),
                                                   ignoredUsers: MockReferenceData.IgnoredUsers,
                                                   milestones: MockReferenceData.TwitchMilestones));

        this._notificationHandler = new TwitchIncomingMessageNotificationHandler(options: options,
                                                                                 twitchChannelManager: this._twitchChannelManager,
                                                                                 twitchCustomMessageHandler: this._twitchCustomMessageHandler,
                                                                                 this.GetTypedLogger<TwitchIncomingMessageNotificationHandler>());
    }

    [Fact]
    public async Task WhenHandledByCustomMessageHandlerDoesNothingElseAsync()
    {
        this.MockCustomMessageHandler(true);

        await this._notificationHandler.Handle(new(Streamer: MockReferenceData.Streamer, Chatter: MockReferenceData.Viewer, Message: "Banana"),
                                               cancellationToken: CancellationToken.None);

        await this.ReceivedCustomMessageHandlerAsync();
        this.DidNotReceiveGetStreamer();
    }

    [Fact]
    public async Task WhenNotModChannelDoesNothingElseAsync()
    {
        this.MockCustomMessageHandler(false);

        await this._notificationHandler.Handle(new(MockReferenceData.Streamer.Next(), Chatter: MockReferenceData.Viewer, Message: "Banana"),
                                               cancellationToken: CancellationToken.None);

        await this.ReceivedCustomMessageHandlerAsync();
        this.DidNotReceiveGetStreamer();
    }

    [Fact]
    public async Task WhenModChannelForwardsChatMessageAsync()
    {
        this.MockCustomMessageHandler(false);

        await this._notificationHandler.Handle(new(Streamer: MockReferenceData.Streamer, Chatter: MockReferenceData.Viewer, Message: "Banana"),
                                               cancellationToken: CancellationToken.None);

        await this.ReceivedCustomMessageHandlerAsync();
        this.ReceivedGetStreamer();

        await this._twitchChannelState.Received(1)
                  .ChatMessageAsync(user: MockReferenceData.Viewer, message: "Banana", Arg.Any<CancellationToken>());
    }

    private void ReceivedGetStreamer()
    {
        this._twitchChannelManager.Received(1)
            .GetStreamer(MockReferenceData.Streamer);
    }

    private void DidNotReceiveGetStreamer()
    {
        this._twitchChannelManager.DidNotReceive()
            .GetStreamer(Arg.Any<Streamer>());
    }

    private Task<bool> ReceivedCustomMessageHandlerAsync()
    {
        return this._twitchCustomMessageHandler.Received(1)
                   .HandleMessageAsync(Arg.Any<TwitchIncomingMessage>(), Arg.Any<CancellationToken>());
    }

    private void MockCustomMessageHandler(bool handled)
    {
        this._twitchCustomMessageHandler.HandleMessageAsync(Arg.Any<TwitchIncomingMessage>(), Arg.Any<CancellationToken>())
            .Returns(handled);
    }
}