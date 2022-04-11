using System;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Database.Dapper.Tests;

public sealed class RetryDelayCalculatorTests : TestBase
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 0)]
    [InlineData(2, 4)]
    [InlineData(3, 8)]
    [InlineData(4, 16)]
    [InlineData(5, 32)]
    public void Calculate(int attempts, int expectedDuration)
    {
        TimeSpan expected = TimeSpan.FromSeconds(expectedDuration);
        TimeSpan actual = RetryDelayCalculator.Calculate(attempts);

        Assert.Equal(expected: expected, actual: actual);
    }
}