using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Configuration;
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
    private readonly ITwitchCustomMessageHandler _twitchCustomMessageHandler;

    public TwitchIncomingMessageNotificationHandlerTests()
    {
        this._twitchCustomMessageHandler = GetSubstitute<ITwitchCustomMessageHandler>();
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(
            new TwitchBotOptions(authentication: MockReferenceData.TwitchAuthentication, [])
        );

        this._notificationHandler = new TwitchIncomingMessageNotificationHandler(
            twitchCustomMessageHandler: this._twitchCustomMessageHandler
        );
    }

    [Fact]
    public async Task WhenHandledByCustomMessageHandlerDoesNothingElseAsync()
    {
        this.MockCustomMessageHandler(true);

        await this._notificationHandler.Handle(
            new(
                Streamer: MockReferenceData.Streamer,
                Chatter: MockReferenceData.Viewer,
                Message: "Banana"
            ),
            cancellationToken: CancellationToken.None
        );

        await this.ReceivedCustomMessageHandlerAsync();
    }

    [Fact]
    public async Task WhenNotModChannelDoesNothingElseAsync()
    {
        this.MockCustomMessageHandler(false);

        await this._notificationHandler.Handle(
            new(
                MockReferenceData.Streamer.Next(),
                Chatter: MockReferenceData.Viewer,
                Message: "Banana"
            ),
            cancellationToken: CancellationToken.None
        );

        await this.ReceivedCustomMessageHandlerAsync();
    }

    private Task<bool> ReceivedCustomMessageHandlerAsync()
    {
        return this
            ._twitchCustomMessageHandler.Received(1)
            .HandleMessageAsync(Arg.Any<TwitchIncomingMessage>(), Arg.Any<CancellationToken>());
    }

    private void MockCustomMessageHandler(bool handled)
    {
        this._twitchCustomMessageHandler.HandleMessageAsync(
                Arg.Any<TwitchIncomingMessage>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(handled);
    }
}
