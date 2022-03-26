using Credfeto.Notification.Bot.Database.Interfaces;
using FunFair.Test.Common;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Credfeto.Notification.Bot.Database.Pgsql.Tests;

public sealed class DependencyInjectionTests : DependencyInjectionTestsBase
{
    public DependencyInjectionTests(ITestOutputHelper output)
        : base(output: output, dependencyInjectionRegistration: Configure)
    {
    }

    private static IServiceCollection Configure(IServiceCollection services)
    {
        return services.AddPostgresql();
    }

    [Fact]
    public void DatabaseMustBeRegistered()
    {
        this.RequireService<IDatabase>();
    }
}