using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Actions.Services;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.StreamState;
using FunFair.Test.Common;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Actions;

public sealed class CustomTriggeredMessageSenderTests : LoggingTestBase
{
    private const string MESSAGE = "!play";

    private readonly ICustomTriggeredMessageSender _sender;
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    public CustomTriggeredMessageSenderTests(ITestOutputHelper output)
        : base(output)
    {
        this._twitchChatMessageChannel = GetSubstitute<IMessageChannel<TwitchChatMessage>>();

        this._sender = new CustomTriggeredMessageSender(
            twitchChatMessageChannel: this._twitchChatMessageChannel,
            logger: this.GetTypedLogger<CustomTriggeredMessageSender>()
        );
    }

    [Fact]
    public async Task SendAsyncShouldPublishToMessageChannel()
    {
        Streamer streamer = MockReferenceData.Streamer;

        await this._sender.SendAsync(streamer: streamer, message: MESSAGE, cancellationToken: this.CancellationToken());

        await this
            ._twitchChatMessageChannel.Received(1)
            .PublishAsync(
                Arg.Is<TwitchChatMessage>(m =>
                    m.Streamer == streamer && m.Message == MESSAGE && m.Priority == MessagePriority.NATURAL
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
