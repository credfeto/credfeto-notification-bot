using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared.Services;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Shared.Tests.Services;

public sealed class MessageChannelTests : TestBase
{
    private readonly IMessageChannel<string> _messageChannel;

    public MessageChannelTests()
    {
        this._messageChannel = new MessageChannel<string>();
    }

    [Fact]
    public async Task PublishedMessageCanBeReceivedAsync()
    {
        const string message = "Hello World";

        await this._messageChannel.PublishAsync(
            message: message,
            cancellationToken: CancellationToken.None
        );

        using (CancellationTokenSource cts = new(TimeSpan.FromSeconds(1)))
        {
            string receivedMessage = await this._messageChannel.ReceiveAsync(cts.Token);
            Assert.Equal(expected: message, actual: receivedMessage);
        }
    }
}
