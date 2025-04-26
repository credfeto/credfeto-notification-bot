using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using Mediator;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Services;

public sealed class TwitchCustomMessageHandlerTests : TestBase
{
    private static readonly TwitchIncomingMessage IncomingMessage = new(
        Streamer: MockReferenceData.Streamer,
        Chatter: MockReferenceData.Viewer,
        Message: "!play"
    );
    private readonly IMediator _mediator;
    private readonly ITwitchCustomMessageHandler _twitchCustomMessageHandler;
    private readonly ITwitchMessageTriggerDebounceFilter _twitchMessageTriggerDebounceFilter;

    public TwitchCustomMessageHandlerTests()
    {
        Streamer streamer = IncomingMessage.Streamer;
        Viewer viewer = IncomingMessage.Chatter;
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(
            new TwitchBotOptions(
                authentication: MockReferenceData.TwitchAuthentication,
                [
                    new(
                        streamer: streamer.Value,
                        bot: viewer.Value,
                        match: "!play",
                        issue: "!play",
                        TwitchMessageMatchType.EXACT.GetName()
                    ),
                ]
            )
        );

        this._mediator = GetSubstitute<IMediator>();
        this._twitchMessageTriggerDebounceFilter = GetSubstitute<ITwitchMessageTriggerDebounceFilter>();

        this._twitchCustomMessageHandler = new TwitchCustomMessageHandler(
            options: options,
            mediator: this._mediator,
            twitchMessageTriggerDebounceFilter: this._twitchMessageTriggerDebounceFilter,
            this.GetTypedLogger<TwitchCustomMessageHandler>()
        );
    }

    [Fact]
    public async Task MatchingMessageShouldNotSendWhenCanSendReturnsFalseAsync()
    {
        this.MockCanSend(false);
        bool handled = await this._twitchCustomMessageHandler.HandleMessageAsync(
            message: IncomingMessage,
            cancellationToken: this.CancellationToken()
        );

        Assert.True(condition: handled, userMessage: "Should have been handled");

        this.ReceivedCanSend();
        await this.DidNotReceiveSendCustomMessageAsync();
    }

    [Fact]
    public async Task NonMatchingMessageShouldNotSendAsync()
    {
        TwitchIncomingMessage misMatchMessage = new(
            Streamer: MockReferenceData.Streamer,
            MockReferenceData.Viewer.Next(),
            Message: "Does Not Match"
        );

        bool handled = await this._twitchCustomMessageHandler.HandleMessageAsync(
            message: misMatchMessage,
            cancellationToken: this.CancellationToken()
        );

        Assert.False(condition: handled, userMessage: "Should not have been handled");

        this.DidNotReceiveCanSend();
        await this.DidNotReceiveSendCustomMessageAsync();
    }

    [Fact]
    public async Task MatchingMessageShouldSendWhenCanSendReturnsTrueAsync()
    {
        this.MockCanSend(true);
        bool handled = await this._twitchCustomMessageHandler.HandleMessageAsync(
            message: IncomingMessage,
            cancellationToken: this.CancellationToken()
        );

        Assert.True(condition: handled, userMessage: "Should have been handled");

        this.ReceivedCanSend();
        await this.ReceivedSendCustomMessageAsync();
    }

    private void ReceivedCanSend()
    {
        this._twitchMessageTriggerDebounceFilter.Received(1).CanSend(Arg.Any<TwitchOutputMessageMatch>());
    }

    private void DidNotReceiveCanSend()
    {
        this._twitchMessageTriggerDebounceFilter.DidNotReceive().CanSend(Arg.Any<TwitchOutputMessageMatch>());
    }

    private ValueTask ReceivedSendCustomMessageAsync()
    {
        return this
            ._mediator.Received(1)
            .Publish(
                Arg.Is<CustomTriggeredMessage>(x => x.Streamer == IncomingMessage.Streamer),
                Arg.Any<CancellationToken>()
            );
    }

    private ValueTask DidNotReceiveSendCustomMessageAsync()
    {
        return this._mediator.DidNotReceive().Publish(Arg.Any<CustomTriggeredMessage>(), Arg.Any<CancellationToken>());
    }

    private void MockCanSend(bool canSend)
    {
        this._twitchMessageTriggerDebounceFilter.CanSend(Arg.Any<TwitchOutputMessageMatch>()).Returns(canSend);
    }
}
