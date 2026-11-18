using System.Collections.Generic;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Server.Helpers;
using FunFair.Test.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;

namespace Credfeto.Notification.Bot.Server.Tests.Helpers;

public sealed class ServerStartupValidatedTwitchOptionsTests : TestBase
{
    private static readonly Dictionary<string, string?> ValidSettings = new(System.StringComparer.Ordinal)
    {
        ["Twitch:Authentication:Chat:UserName"] = "bot-user",
        ["Twitch:Authentication:Chat:OAuthToken"] = "oauth-token",
        ["Twitch:Authentication:Api:ClientId"] = "client-id",
        ["Twitch:Authentication:Api:ClientSecret"] = "client-secret",
    };

    [Fact]
    public async Task HostStartsWithValidTwitchConfigurationAsync()
    {
        using (IHost host = BuildHost(ValidSettings))
        {
            await host.StartAsync(this.CancellationToken());
            await host.StopAsync(this.CancellationToken());
        }
    }

    [Fact]
    public async Task HostFailsToStartWithInvalidTwitchConfigurationAsync()
    {
        Dictionary<string, string?> settings = new(ValidSettings, System.StringComparer.Ordinal)
        {
            ["Twitch:Authentication:Chat:UserName"] = string.Empty,
        };

        using (IHost host = BuildHost(settings))
        {
            await Assert.ThrowsAsync<OptionsValidationException>(() => host.StartAsync(this.CancellationToken()));
        }
    }

    private static IHost BuildHost(IEnumerable<KeyValuePair<string, string?>> settings)
    {
        HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(new HostApplicationBuilderSettings());
        builder.Configuration.AddInMemoryCollection(settings);
        builder.Services.AddValidatedTwitchOptions(builder.Configuration.GetSection("Twitch"));

        return builder.Build();
    }
}
