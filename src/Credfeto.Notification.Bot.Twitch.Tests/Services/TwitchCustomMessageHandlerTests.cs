using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using MediatR;
using Microsoft.Extensions.Options;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Services;

public sealed class TwitchCustomMessageHandlerTests : TestBase
{
    private static readonly TwitchIncomingMessage IncomingMessage = new(Streamer: MockReferenceData.Streamer, Chatter: MockReferenceData.Viewer, Message: "message");
    private readonly IMediator _mediator;
    private readonly ITwitchCustomMessageHandler _twitchCustomMessageHandler;
    private readonly ITwitchMessageTriggerDebounceFilter _twitchMessageTriggerDebounceFilter;

    public TwitchCustomMessageHandlerTests()
    {
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();

        this._mediator = GetSubstitute<IMediator>();
        this._twitchMessageTriggerDebounceFilter = GetSubstitute<ITwitchMessageTriggerDebounceFilter>();

        this._twitchCustomMessageHandler = new TwitchCustomMessageHandler(options: options,
                                                                          mediator: this._mediator,
                                                                          twitchMessageTriggerDebounceFilter: this._twitchMessageTriggerDebounceFilter,
                                                                          this.GetTypedLogger<TwitchCustomMessageHandler>());
    }

    [Fact]
    public async Task ShouldNotDoAnythingVeryUsefulAtAllAsync()
    {
        bool gubbins = await this._twitchCustomMessageHandler.HandleMessageAsync(message: IncomingMessage, cancellationToken: CancellationToken.None);

        Assert.False(condition: gubbins, userMessage: "This needs implementing!");
    }
}