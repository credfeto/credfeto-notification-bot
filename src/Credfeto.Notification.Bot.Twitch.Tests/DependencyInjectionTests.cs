using Credfeto.Date.Interfaces;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.StreamState;
using FunFair.Test.Common;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests;

public sealed class DependencyInjectionTests : DependencyInjectionTestsBase
{
    public DependencyInjectionTests(ITestOutputHelper output)
        : base(output: output, dependencyInjectionRegistration: Configure) { }

    private static IServiceCollection Configure(IServiceCollection service)
    {
        return service
            .AddMockedService<ICurrentTimeSource>()
            .AddMockedService<IMessageChannel<TwitchChatMessage>>()
            .AddMockedService<IOptions<TwitchBotOptions>>(options =>
            {
                options.Value.Returns(
                    new TwitchBotOptions { Authentication = MockReferenceData.TwitchAuthentication, ChatCommands = [] }
                );
            })
            .AddMockedService<IMediator>()
            // Items being tested
            .AddTwitch();
    }

    [Fact]
    public void TwitchChatMustBeRegistered()
    {
        this.RequireService<ITwitchChat>();
    }

    [Fact]
    public void TwitchStreamStatusMustBeRegistered()
    {
        this.RequireService<ITwitchStreamStatus>();
    }
}
