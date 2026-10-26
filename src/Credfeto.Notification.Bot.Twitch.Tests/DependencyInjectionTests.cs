using Credfeto.Date.Interfaces;
using Credfeto.Notification.Bot.Mocks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.StreamState;
using FunFair.Test.Common;
using FunFair.Test.Infrastructure;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using NSubstitute;
using TwitchLib.Communication.Interfaces;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests;

public sealed class DependencyInjectionTests : DependencyInjectionTestsBase
{
    public DependencyInjectionTests(ITestOutputHelper output)
        : base(output: output, dependencyInjectionRegistration: Configure) { }

    private static IServiceCollection Configure(IServiceCollection service)
    {
        return ReplaceWebSocketClientWithOfflineClient(
            service
                .AddMockedService<ICurrentTimeSource>()
                .AddMockedService<IMessageChannel<TwitchChatMessage>>()
                .AddMockedService<IOptions<TwitchBotOptions>>(options =>
                {
                    options.Value.Returns(
                        new TwitchBotOptions
                        {
                            Authentication = MockReferenceData.TwitchAuthentication,
                            ChatCommands = [],
                        }
                    );
                })
                .AddMockedService<IMediator>()
                // Items being tested
                .AddTwitch()
        );
    }

    private static IServiceCollection ReplaceWebSocketClientWithOfflineClient(IServiceCollection services)
    {
        return services
            .RemoveAll<IClient>()
            .AddMockedService<IClient>(client =>
            {
                client.IsConnected.Returns(true);
                client.OpenAsync().Returns(true);
                client.SendAsync(Arg.Any<string>()).Returns(true);
            });
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
