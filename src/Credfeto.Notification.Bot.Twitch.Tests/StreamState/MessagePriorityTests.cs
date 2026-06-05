using System.Diagnostics;
using Credfeto.Notification.Bot.Twitch.StreamState;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.StreamState;

public sealed class MessagePriorityTests : TestBase
{
    [Theory]
    [InlineData(MessagePriority.ASAP, "ASAP")]
    [InlineData(MessagePriority.NATURAL, "NATURAL")]
    [InlineData(MessagePriority.SLOW, "SLOW")]
    public void GetNameShouldReturnCorrectName(MessagePriority priority, string expectedName)
    {
        string name = priority.GetName();

        Assert.Equal(expected: expectedName, actual: name);
    }

    [Fact]
    public void GetNameForUnknownValueShouldThrow()
    {
        const MessagePriority unknown = (MessagePriority)999;

        Assert.Throws<UnreachableException>(() => unknown.GetName());
    }

    [Fact]
    public void GetDescriptionShouldReturnName()
    {
        Assert.Equal(expected: "ASAP", actual: MessagePriority.ASAP.GetDescription());
    }

    [Theory]
    [InlineData(MessagePriority.ASAP)]
    [InlineData(MessagePriority.NATURAL)]
    [InlineData(MessagePriority.SLOW)]
    public void IsDefinedShouldReturnTrueForKnownValues(MessagePriority priority)
    {
        bool isDefined = priority.IsDefined();

        Assert.True(condition: isDefined, userMessage: "Known priority should be defined");
    }

    [Fact]
    public void UnknownValueShouldNotBeDefined()
    {
        const MessagePriority unknown = (MessagePriority)999;

        bool isDefined = unknown.IsDefined();

        Assert.False(condition: isDefined, userMessage: "Unknown priority should not be defined");
    }
}
