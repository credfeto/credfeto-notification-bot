using Credfeto.Notification.Bot.Database.Interfaces.Builders;
using Credfeto.Notification.Bot.Database.Shared.Tests.Mocks;
using FunFair.Test.Common;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Credfeto.Notification.Bot.Database.Shared.Tests;

public sealed class DependencyInjectionTests : DependencyInjectionTestsBase
{
    public DependencyInjectionTests(ITestOutputHelper output)
        : base(output: output, dependencyInjectionRegistration: Configure)
    {
    }

    private static IServiceCollection Configure(IServiceCollection services)
    {
        return services.AddMockedService<IObjectBuilder<BananaEntity, Banana>>()
                       .AddDatabaseShared();
    }

    [Fact]
    public void ObjectCollectionBuilderMustBeRegistered()
    {
        this.RequireService<IObjectCollectionBuilder<BananaEntity, Banana>>();
    }
}