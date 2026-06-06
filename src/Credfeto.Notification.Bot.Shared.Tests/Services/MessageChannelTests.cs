using System;
using System.Collections.Generic;
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

        await this._messageChannel.PublishAsync(message: message, cancellationToken: this.CancellationToken());

        using (CancellationTokenSource cts = new(TimeSpan.FromSeconds(1)))
        {
            string receivedMessage = await this._messageChannel.ReceiveAsync(cts.Token);
            Assert.Equal(expected: message, actual: receivedMessage);
        }
    }

    [Fact]
    public async Task PublishedMessageCanBeReceivedViaReadAllAsync()
    {
        const string message = "Hello World";

        await this._messageChannel.PublishAsync(message: message, cancellationToken: this.CancellationToken());

        IAsyncEnumerator<string> enumerator = this
            ._messageChannel.ReadAllAsync(this.CancellationToken())
            .GetAsyncEnumerator(this.CancellationToken());

        await using (enumerator)
        {
            bool hasNext = await enumerator.MoveNextAsync();
            Assert.True(condition: hasNext, userMessage: "Expected a message to be available in the channel");
            Assert.Equal(expected: message, actual: enumerator.Current);
        }
    }
}
