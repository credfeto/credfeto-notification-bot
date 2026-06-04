using Credfeto.Notification.Bot.Server.Helpers;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Server.Tests.Helpers;

public sealed class StartupBannerShowTests : TestBase
{
    [Fact]
    public void ShowDoesNotThrow()
    {
        StartupBanner.Show();
    }
}
