using System;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using FunFair.Test.Common;
using Xunit.Abstractions;

namespace Credfeto.Notification.Bot.Database.Tests.Integration.Setup;

public abstract class DatabaseIntegrationTestBase : IntegrationTestBase
{
    protected DatabaseIntegrationTestBase(ITestOutputHelper output)
        : base(output: output, dependencyInjectionRegistration: IntegrationTestStartup.ConfigureServices)
    {
    }

    protected static Streamer GenerateStreamerUsername()
    {
        return new("@S" + Guid.NewGuid()
                              .ToString()
                              .Replace(oldValue: "-", newValue: "")
                              .ToLowerInvariant());
    }

    protected static Viewer GenerateViewerUsername()
    {
        return new("@V" + Guid.NewGuid()
                              .ToString()
                              .Replace(oldValue: "-", newValue: "")
                              .ToLowerInvariant());
    }
}