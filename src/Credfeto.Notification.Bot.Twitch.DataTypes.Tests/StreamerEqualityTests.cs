using Credfeto.Notification.Bot.Twitch.DataTypes;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.DataTypes.Tests;

public sealed class StreamerEqualityTests : TestBase
{
    private static readonly Streamer TestStreamer = Streamer.FromString("teststreamer");
    private static readonly Streamer OtherStreamer = Streamer.FromString("anotherstreamer");

    [Fact]
    public void Equals_SameValue_IsTrue()
    {
        Streamer other = Streamer.FromString("teststreamer");

        Assert.True(TestStreamer.Equals(other), userMessage: "Streamers with the same value should be equal");
    }

    [Fact]
    public void Equals_DifferentValue_IsFalse()
    {
        Assert.False(
            TestStreamer.Equals(OtherStreamer),
            userMessage: "Streamers with different values should not be equal"
        );
    }

    [Fact]
    public void Equals_Object_SameValue_IsTrue()
    {
        object other = Streamer.FromString("teststreamer");

        Assert.True(
            TestStreamer.Equals(other),
            userMessage: "Streamer should equal boxed Streamer with the same value"
        );
    }

    [Fact]
    public void Equals_Object_DifferentType_IsFalse()
    {
        object? stringObject = "teststreamer";

        Assert.False(TestStreamer.Equals(stringObject), userMessage: "Streamer should not equal a string");
    }

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHash()
    {
        Streamer other = Streamer.FromString("teststreamer");

        Assert.Equal(expected: TestStreamer.GetHashCode(), actual: other.GetHashCode());
    }

    [Fact]
    public void OperatorEquals_SameValue_IsTrue()
    {
        Streamer other = Streamer.FromString("teststreamer");

        Assert.True(TestStreamer == other, userMessage: "Streamers with the same value should be equal via ==");
    }

    [Fact]
    public void OperatorNotEquals_DifferentValue_IsTrue()
    {
        Assert.True(
            TestStreamer != OtherStreamer,
            userMessage: "Streamers with different values should not be equal via !="
        );
    }

    [Fact]
    public void CompareTo_SameValue_ReturnsZero()
    {
        Streamer other = Streamer.FromString("teststreamer");

        Assert.Equal(expected: 0, actual: TestStreamer.CompareTo(other));
    }

    [Fact]
    public void CompareTo_DefaultValues_ReturnsZero()
    {
        Streamer empty1 = default;
        Streamer empty2 = default;

        Assert.Equal(expected: 0, actual: empty1.CompareTo(empty2));
    }

    [Fact]
    public void CompareTo_DefaultVsNonDefault_ReturnsNegative()
    {
        Streamer empty = default;

        Assert.True(
            empty.CompareTo(TestStreamer) < 0,
            userMessage: "Default Streamer should compare less than non-default"
        );
    }

    [Fact]
    public void CompareTo_NonDefaultVsDefault_ReturnsPositive()
    {
        Streamer empty = default;

        Assert.True(
            TestStreamer.CompareTo(empty) > 0,
            userMessage: "Non-default Streamer should compare greater than default"
        );
    }

    [Fact]
    public void CompareTo_SmallerValue_ReturnsPositive()
    {
        Assert.True(
            TestStreamer.CompareTo(OtherStreamer) > 0,
            userMessage: "CompareTo should return positive when compared to a smaller value"
        );
    }

    [Fact]
    public void CompareTo_LargerValue_ReturnsNegative()
    {
        Assert.True(
            OtherStreamer.CompareTo(TestStreamer) < 0,
            userMessage: "CompareTo should return negative when compared to a larger value"
        );
    }

    [Fact]
    public void OperatorLessThan_SmallerToLarger_IsTrue()
    {
        Assert.True(OtherStreamer < TestStreamer, userMessage: "Smaller streamer should be less than larger");
    }

    [Fact]
    public void OperatorGreaterThan_LargerToSmaller_IsTrue()
    {
        Assert.True(TestStreamer > OtherStreamer, userMessage: "Larger streamer should be greater than smaller");
    }

    [Fact]
    public void OperatorLessThanOrEqual_SameValue_IsTrue()
    {
        Streamer other = Streamer.FromString("teststreamer");

        Assert.True(TestStreamer <= other, userMessage: "Equal streamers should satisfy <=");
    }

    [Fact]
    public void OperatorGreaterThanOrEqual_SameValue_IsTrue()
    {
        Streamer other = Streamer.FromString("teststreamer");

        Assert.True(TestStreamer >= other, userMessage: "Equal streamers should satisfy >=");
    }
}
