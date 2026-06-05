using System.Threading.Tasks;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using Mediator;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Services;

public sealed class TwitchStreamStatusTests : LoggingTestBase
{
    private readonly TwitchStreamStatus _twitchStreamStatus;

    public TwitchStreamStatusTests(ITestOutputHelper output)
        : base(output)
    {
        IOptions<TwitchBotOptions> options = GetSubstitute<IOptions<TwitchBotOptions>>();
        options.Value.Returns(
            new TwitchBotOptions { Authentication = MockReferenceData.TwitchAuthentication, ChatCommands = [] }
        );

        IMediator mediator = GetSubstitute<IMediator>();

        this._twitchStreamStatus = new TwitchStreamStatus(
            options: options,
            mediator: mediator,
            logger: this.GetTypedLogger<TwitchStreamStatus>()
        );
    }

    [Fact]
    public void CanBeConstructedAndDisposed()
    {
        this._twitchStreamStatus.Dispose();
    }

    [Fact]
    public Task UpdateAsyncWithNoChannelsShouldReturnImmediately()
    {
        return this._twitchStreamStatus.UpdateAsync();
    }
}
