using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Actions.Services;
using Credfeto.Notification.Bot.Twitch.StreamState;
using FunFair.Test.Common;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Actions.Services;

public sealed class CustomTriggeredMessageSenderTests : TestBase
{
    private readonly ICustomTriggeredMessageSender _customTriggeredMessageSender;
    private readonly IMessageChannel<TwitchChatMessage> _twitchChatMessageChannel;

    public CustomTriggeredMessageSenderTests()
    {
        this._twitchChatMessageChannel = GetSubstitute<IMessageChannel<TwitchChatMessage>>();

        this._customTriggeredMessageSender = new CustomTriggeredMessageSender(twitchChatMessageChannel: this._twitchChatMessageChannel, this.GetTypedLogger<CustomTriggeredMessageSender>());
    }

    [Fact]
    public async Task JoinHeistAsync()
    {
        await this._customTriggeredMessageSender.JoinMarblesAsync(streamer: MockReferenceData.Streamer, cancellationToken: CancellationToken.None);

        await this.ReceivedPublishMessageAsync("!play");
    }

    private ValueTask ReceivedPublishMessageAsync(string expectedMessage)
    {
        return this._twitchChatMessageChannel.Received(1)
                   .PublishAsync(Arg.Is<TwitchChatMessage>(t => t.Streamer == MockReferenceData.Streamer && t.Message == expectedMessage), Arg.Any<CancellationToken>());
    }
}