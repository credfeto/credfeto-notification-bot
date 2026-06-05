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

public sealed class TwitchCustomMessageHandlerEndsWithTests : LoggingTestBase
{
    private static readonly TwitchIncomingMessage MatchingMessage = new(
        Streamer: MockReferenceData.Streamer,
        Chatter: MockReferenceData.Viewer,
        Message: "please !play"
    );

    private readonly IMediator _mediator;
    private readonly ITwitchCustomMessageHandler _twitchCustomMessageHandler;
    private readonly ITwitchMessageTriggerDebounceFilter _twitchMessageTriggerDebounceFilter;

    public TwitchCustomMessageHandlerEndsWithTests(ITestOutputHelper output)
        : base(output)
    {
        Streamer streamer = MatchingMessage.Streamer;
        Viewer viewer = MatchingMessage.Chatter;
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(
            new TwitchBotOptions
            {
                Authentication = MockReferenceData.TwitchAuthentication,
                ChatCommands =
                [
                    new(
                        streamer: streamer.Value,
                        bot: viewer.Value,
                        match: "!play",
                        issue: "!play",
                        TwitchMessageMatchType.ENDS_WITH.GetName()
                    ),
                ],
            }
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
    public async Task EndsWithMatchShouldSendWhenCanSendReturnsTrueAsync()
    {
        this._twitchMessageTriggerDebounceFilter.CanSend(Arg.Any<TwitchOutputMessageMatch>()).Returns(true);

        bool handled = await this._twitchCustomMessageHandler.HandleMessageAsync(
            message: MatchingMessage,
            this.CancellationToken()
        );

        Assert.True(condition: handled, userMessage: "Message should have been handled");
        await this._mediator.Received(1).Publish(Arg.Any<CustomTriggeredMessage>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task NonEndsWithMessageShouldNotMatchAsync()
    {
        TwitchIncomingMessage noMatchMessage = new(
            Streamer: MockReferenceData.Streamer,
            Chatter: MockReferenceData.Viewer,
            Message: "!play then something"
        );

        bool handled = await this._twitchCustomMessageHandler.HandleMessageAsync(
            message: noMatchMessage,
            this.CancellationToken()
        );

        Assert.False(condition: handled, userMessage: "Message should not have been handled");
        await this._mediator.DidNotReceive().Publish(Arg.Any<CustomTriggeredMessage>(), Arg.Any<CancellationToken>());
    }
}
