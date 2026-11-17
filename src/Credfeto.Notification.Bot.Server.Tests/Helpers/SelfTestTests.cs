using System;
using System.IO;
using Credfeto.Notification.Bot.Server.Helpers;
using FunFair.Test.Common;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Credfeto.Notification.Bot.Server.Tests.Helpers;

public sealed class SelfTestTests : TestBase
{
    [Fact]
    public void IsRequestedWhenSwitchPresent()
    {
        Assert.True(SelfTest.IsRequested(["--other", "--self-test"]), userMessage: "Switch should be detected");
    }

    [Fact]
    public void IsNotRequestedWhenSwitchAbsent()
    {
        Assert.False(SelfTest.IsRequested(["--other", "--SELF-TEST"]), userMessage: "Switch is case-sensitive");
    }

    [Fact]
    public void WithoutSwitchRemovesOnlyTheSwitch()
    {
        Assert.Equal(
            expected: ["--other", "value"],
            actual: SelfTest.WithoutSwitch(["--other", "--self-test", "value"])
        );
    }

    [Fact]
    public void RunSucceedsWithRealApplication()
    {
        using (StringWriter output = new())
        {
            int exitCode = SelfTest.Run(createApp: () => ServerStartup.CreateSelfTestApp([]), output: output);

            Assert.Equal(expected: SelfTest.SUCCESS, actual: exitCode);
            Assert.Equal(expected: "Self-test passed" + Environment.NewLine, actual: output.ToString());
        }
    }

    [Fact]
    public void RunFailsWithOneLineReasonWhenApplicationCannotBeCreated()
    {
        using (StringWriter output = new())
        {
            int exitCode = SelfTest.Run(createApp: FailToCreateApp, output: output);

            Assert.Equal(expected: SelfTest.FAILURE, actual: exitCode);
            Assert.Equal(
                expected: "Self-test failed: InvalidOperationException: first line second line" + Environment.NewLine,
                actual: output.ToString()
            );
        }
    }

    private static IHost FailToCreateApp()
    {
        throw new InvalidOperationException("first line" + Environment.NewLine + "second line");
    }
}
