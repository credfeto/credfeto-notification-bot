using System.Diagnostics;
using Credfeto.Notification.Bot.Twitch.Models;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Models.Tests;

public sealed class TwitchMessageMatchTypeTests : TestBase
{
    [Theory]
    [InlineData(TwitchMessageMatchType.EXACT, "EXACT")]
    [InlineData(TwitchMessageMatchType.STARTS_WITH, "STARTS_WITH")]
    [InlineData(TwitchMessageMatchType.ENDS_WITH, "ENDS_WITH")]
    [InlineData(TwitchMessageMatchType.CONTAINS, "CONTAINS")]
    [InlineData(TwitchMessageMatchType.REGEX, "REGEX")]
    public void GetName_ReturnsExpectedName(TwitchMessageMatchType matchType, string expectedName)
    {
        Assert.Equal(expected: expectedName, actual: matchType.GetName());
    }

    [Fact]
    public void GetName_ThrowsForUndefinedValue()
    {
        const TwitchMessageMatchType undefined = (TwitchMessageMatchType)99;
        Assert.Throws<UnreachableException>(() => undefined.GetName());
    }

    [Fact]
    public void GetDescription_ReturnsName()
    {
        Assert.Equal(expected: "EXACT", actual: TwitchMessageMatchType.EXACT.GetDescription());
    }

    [Theory]
    [InlineData(TwitchMessageMatchType.EXACT, true)]
    [InlineData(TwitchMessageMatchType.STARTS_WITH, true)]
    [InlineData(TwitchMessageMatchType.ENDS_WITH, true)]
    [InlineData(TwitchMessageMatchType.CONTAINS, true)]
    [InlineData(TwitchMessageMatchType.REGEX, true)]
    public void IsDefined_ReturnsTrueForValidValues(TwitchMessageMatchType matchType, bool expected)
    {
        Assert.Equal(expected: expected, actual: matchType.IsDefined());
    }

    [Fact]
    public void IsDefined_ReturnsFalseForUndefinedValue()
    {
        const TwitchMessageMatchType undefined = (TwitchMessageMatchType)99;
        Assert.False(
            condition: undefined.IsDefined(),
            userMessage: "IsDefined should return false for undefined enum values"
        );
    }
}
