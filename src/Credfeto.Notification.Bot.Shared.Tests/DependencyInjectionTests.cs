using FunFair.Test.Common;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Credfeto.Notification.Bot.Shared.Tests;

public sealed class DependencyInjectionTests : DependencyInjectionTestsBase
{
    public DependencyInjectionTests(ITestOutputHelper output)
        : base(output: output, dependencyInjectionRegistration: Configure) { }

    private static IServiceCollection Configure(IServiceCollection services)
    {
        return services.AddResources();
    }

    [Fact]
    public void MessageChannelMustBeRegistered()
    {
        this.RequireService<IMessageChannel<string>>();
    }
}
