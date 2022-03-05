using System;
using FunFair.Test.Common;
using Xunit.Abstractions;

namespace Credfeto.Notification.Bot.Database.Tests.Integration.Setup;

public abstract class DatabaseIntegrationTestBase : IntegrationTestBase
{
    protected DatabaseIntegrationTestBase(ITestOutputHelper output)
        : base(output: output, dependencyInjectionRegistration: IntegrationTestStartup.ConfigureServices)
    {
    }

    protected static string GenerateUsername()
    {
        return "@" + Guid.NewGuid()
                         .ToString()
                         .Replace(oldValue: "-", newValue: "");
    }
}