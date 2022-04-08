using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Actions;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.StreamState;
using FunFair.Test.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace Credfeto.Notification.Bot.Twitch.Tests;

public sealed class DependencyInjectionTests : DependencyInjectionTestsBase
{
    public DependencyInjectionTests(ITestOutputHelper output)
        : base(output: output, dependencyInjectionRegistration: Configure)
    {
    }

    private static IServiceCollection Configure(IServiceCollection service)
    {
        return service.AddMockedService<ICurrentTimeSource>()
                      .AddMockedService<IMessageChannel<TwitchChatMessage>>()
                      .AddMockedService<IOptions<TwitchBotOptions>>(options =>
                                                                    {
                                                                        options.Value.Returns(new TwitchBotOptions
                                                                                              {
                                                                                                  Authentication = new()
                                                                                                                   {
                                                                                                                       ClientId = "Invalid",
                                                                                                                       ClientAccessToken = "Invalid",
                                                                                                                       ClientSecret = "Invalid",
                                                                                                                       OAuthToken = "Invalid",
                                                                                                                       UserName = "Invalid"
                                                                                                                   },
                                                                                                  Channels = new(),
                                                                                                  Heists = new()
                                                                                              });
                                                                    })
                      .AddMockedService<IMediator>()
                      .AddMockedService<ITwitchStreamDataManager>()
                      .AddMockedService<ITwitchStreamerDataManager>()

                      // Items being tested
                      .AddTwitch();
    }

    [Fact]
    public void UserInfoServiceMustBeRegistered()
    {
        this.RequireService<IUserInfoService>();
    }

    [Fact]
    public void RaidWelcomeMustBeRegistered()
    {
        this.RequireService<IRaidWelcome>();
    }

    [Fact]
    public void HeistJoinerMustBeRegistered()
    {
        this.RequireService<IHeistJoiner>();
    }

    [Fact]
    public void ShoutoutJoinerMustBeRegistered()
    {
        this.RequireService<IShoutoutJoiner>();
    }

    [Fact]
    public void ContributionThanksMustBeRegistered()
    {
        this.RequireService<IContributionThanks>();
    }

    [Fact]
    public void WelcomeWaggonMustBeRegistered()
    {
        this.RequireService<IWelcomeWaggon>();
    }

    [Fact]
    public void TwitchChannelManagerMustBeRegistered()
    {
        this.RequireService<ITwitchChannelManager>();
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

    [Fact]
    public void ChannelFollowCountMustBeRegistered()
    {
        this.RequireService<IChannelFollowCount>();
    }

    [Fact]
    public void FollowerMilestoneMustBeRegistered()
    {
        this.RequireService<IFollowerMilestone>();
    }
}