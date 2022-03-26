using System.Linq;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Shared.Extensions.Tests;

public sealed class EnumerableExtensionsTests : TestBase
{
    [Fact]
    public void RemoveNulls()
    {
        object[] expected =
        {
            1,
            2,
            3
        };
        object?[] source =
        {
            1,
            null,
            2,
            3
        };
        object[] actual = source.RemoveNulls()
                                .ToArray();

        Assert.Equal(expected: expected, actual: actual);
    }
}