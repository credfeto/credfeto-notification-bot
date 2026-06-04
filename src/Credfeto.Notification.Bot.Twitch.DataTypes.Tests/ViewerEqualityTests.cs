using Credfeto.Notification.Bot.Twitch.DataTypes;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.DataTypes.Tests;

public sealed class ViewerEqualityTests : TestBase
{
    private static readonly Viewer TestViewer = Viewer.FromString("testviewer");
    private static readonly Viewer OtherViewer = Viewer.FromString("anotherviewer");

    [Fact]
    public void Equals_SameValue_IsTrue()
    {
        Viewer other = Viewer.FromString("testviewer");

        Assert.True(TestViewer.Equals(other), userMessage: "Viewers with the same value should be equal");
    }

    [Fact]
    public void Equals_DifferentValue_IsFalse()
    {
        Assert.False(TestViewer.Equals(OtherViewer), userMessage: "Viewers with different values should not be equal");
    }

    [Fact]
    public void Equals_Object_SameValue_IsTrue()
    {
        object other = Viewer.FromString("testviewer");

        Assert.True(TestViewer.Equals(other), userMessage: "Viewer should equal boxed Viewer with the same value");
    }

    [Fact]
    public void Equals_Object_DifferentType_IsFalse()
    {
        object? stringObject = "testviewer";

        Assert.False(TestViewer.Equals(stringObject), userMessage: "Viewer should not equal a string");
    }

    [Fact]
    public void GetHashCode_SameValue_ReturnsSameHash()
    {
        Viewer other = Viewer.FromString("testviewer");

        Assert.Equal(expected: TestViewer.GetHashCode(), actual: other.GetHashCode());
    }

    [Fact]
    public void OperatorEquals_SameValue_IsTrue()
    {
        Viewer other = Viewer.FromString("testviewer");

        Assert.True(TestViewer == other, userMessage: "Viewers with the same value should be equal via ==");
    }

    [Fact]
    public void OperatorNotEquals_DifferentValue_IsTrue()
    {
        Assert.True(TestViewer != OtherViewer, userMessage: "Viewers with different values should not be equal via !=");
    }

    [Fact]
    public void CompareTo_SameValue_ReturnsZero()
    {
        Viewer other = Viewer.FromString("testviewer");

        Assert.Equal(expected: 0, actual: TestViewer.CompareTo(other));
    }

    [Fact]
    public void CompareTo_DefaultValues_ReturnsZero()
    {
        Viewer empty1 = default;
        Viewer empty2 = default;

        Assert.Equal(expected: 0, actual: empty1.CompareTo(empty2));
    }

    [Fact]
    public void CompareTo_DefaultVsNonDefault_ReturnsNegative()
    {
        Viewer empty = default;

        Assert.True(
            empty.CompareTo(TestViewer) < 0,
            userMessage: "Default Viewer should compare less than non-default"
        );
    }

    [Fact]
    public void CompareTo_NonDefaultVsDefault_ReturnsPositive()
    {
        Viewer empty = default;

        Assert.True(
            TestViewer.CompareTo(empty) > 0,
            userMessage: "Non-default Viewer should compare greater than default"
        );
    }

    [Fact]
    public void CompareTo_SmallerValue_ReturnsPositive()
    {
        Assert.True(
            TestViewer.CompareTo(OtherViewer) > 0,
            userMessage: "CompareTo should return positive when compared to a smaller value"
        );
    }

    [Fact]
    public void CompareTo_LargerValue_ReturnsNegative()
    {
        Assert.True(
            OtherViewer.CompareTo(TestViewer) < 0,
            userMessage: "CompareTo should return negative when compared to a larger value"
        );
    }

    [Fact]
    public void OperatorLessThan_SmallerToLarger_IsTrue()
    {
        Assert.True(OtherViewer < TestViewer, userMessage: "Smaller viewer should be less than larger");
    }

    [Fact]
    public void OperatorGreaterThan_LargerToSmaller_IsTrue()
    {
        Assert.True(TestViewer > OtherViewer, userMessage: "Larger viewer should be greater than smaller");
    }

    [Fact]
    public void OperatorLessThanOrEqual_SameValue_IsTrue()
    {
        Viewer other = Viewer.FromString("testviewer");

        Assert.True(TestViewer <= other, userMessage: "Equal viewers should satisfy <=");
    }

    [Fact]
    public void OperatorGreaterThanOrEqual_SameValue_IsTrue()
    {
        Viewer other = Viewer.FromString("testviewer");

        Assert.True(TestViewer >= other, userMessage: "Equal viewers should satisfy >=");
    }
}
