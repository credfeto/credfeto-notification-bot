using Credfeto.Notification.Bot.Server.Helpers;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Server.Tests.Helpers;

public sealed class ApplicationConfigLocatorTests : TestBase
{
    [Fact]
    public void ConfigurationFilesPathIsNotNullOrWhiteSpace()
    {
        string path = ApplicationConfigLocator.ConfigurationFilesPath;

        Assert.False(
            condition: string.IsNullOrWhiteSpace(path),
            userMessage: "ConfigurationFilesPath should not be null, empty, or whitespace"
        );
    }
}
