using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Models.Tests;

public sealed class TwitchOutputMessageMatchTests : TestBase
{
    private static readonly Streamer TEST_STREAMER = Streamer.FromString("teststreamer");
    private static readonly Streamer OTHER_STREAMER = Streamer.FromString("otherstreamer");
    private const string TEST_MESSAGE = "Hello world";
    private const string OTHER_MESSAGE = "Goodbye world";

    private static TwitchOutputMessageMatch? GetNullMatch() => null;

    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        TwitchOutputMessageMatch match = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);

        Assert.Equal(expected: TEST_STREAMER, actual: match.Streamer);
        Assert.Equal(expected: TEST_MESSAGE, actual: match.Message);
    }

    [Fact]
    public void Equals_Typed_WithNull_ReturnsFalse()
    {
        TwitchOutputMessageMatch match = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);

        Assert.False(condition: match.Equals(GetNullMatch()), userMessage: "Typed Equals should return false for null");
    }

    [Fact]
    public void Equals_Typed_WithSameReference_ReturnsTrue()
    {
        TwitchOutputMessageMatch match = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);

        Assert.True(condition: match.Equals(match), userMessage: "Typed Equals should return true for same reference");
    }

    [Fact]
    public void Equals_Typed_WithEqualValues_ReturnsTrue()
    {
        TwitchOutputMessageMatch first = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);
        TwitchOutputMessageMatch second = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);

        Assert.True(condition: first.Equals(second), userMessage: "Typed Equals should return true for equal values");
    }

    [Fact]
    public void Equals_Typed_WithDifferentStreamer_ReturnsFalse()
    {
        TwitchOutputMessageMatch first = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);
        TwitchOutputMessageMatch second = new(streamer: OTHER_STREAMER, message: TEST_MESSAGE);

        Assert.False(
            condition: first.Equals(second),
            userMessage: "Typed Equals should return false for different streamers"
        );
    }

    [Fact]
    public void Equals_Typed_WithDifferentMessage_ReturnsFalse()
    {
        TwitchOutputMessageMatch first = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);
        TwitchOutputMessageMatch second = new(streamer: TEST_STREAMER, message: OTHER_MESSAGE);

        Assert.False(
            condition: first.Equals(second),
            userMessage: "Typed Equals should return false for different messages"
        );
    }

    [Fact]
    public void Equals_Object_WithNull_ReturnsFalse()
    {
        TwitchOutputMessageMatch match = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);
        object? nullObject = GetNullMatch();

        Assert.False(condition: match.Equals(nullObject), userMessage: "Equals(object) should return false for null");
    }

    [Fact]
    public void Equals_Object_WithSameReference_ReturnsTrue()
    {
        TwitchOutputMessageMatch match = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);

        Assert.True(
            condition: match.Equals((object)match),
            userMessage: "Equals(object) should return true for same reference"
        );
    }

    [Fact]
    public void Equals_Object_WithEqualObject_ReturnsTrue()
    {
        TwitchOutputMessageMatch first = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);
        TwitchOutputMessageMatch second = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);

        Assert.True(
            condition: first.Equals((object)second),
            userMessage: "Equals(object) should return true for equal objects"
        );
    }

    [Fact]
    public void Equals_Object_WithUnequalObject_ReturnsFalse()
    {
        TwitchOutputMessageMatch first = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);
        TwitchOutputMessageMatch second = new(streamer: OTHER_STREAMER, message: TEST_MESSAGE);

        Assert.False(
            condition: first.Equals((object)second),
            userMessage: "Equals(object) should return false for unequal objects"
        );
    }

    [Fact]
    public void Equals_Object_WithDifferentType_ReturnsFalse()
    {
        TwitchOutputMessageMatch match = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);

        Assert.False(
            condition: match.Equals("not a TwitchOutputMessageMatch"),
            userMessage: "Equals(object) should return false for different types"
        );
    }

    [Fact]
    public void GetHashCode_IsConsistentForEqualValues()
    {
        TwitchOutputMessageMatch first = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);
        TwitchOutputMessageMatch second = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);

        Assert.Equal(expected: first.GetHashCode(), actual: second.GetHashCode());
    }

    [Fact]
    public void OperatorEqual_BothNull_ReturnsTrue()
    {
        TwitchOutputMessageMatch? left = GetNullMatch();
        TwitchOutputMessageMatch? right = GetNullMatch();

        Assert.True(condition: left == right, userMessage: "Two null values should be equal via ==");
    }

    [Fact]
    public void OperatorEqual_LeftNull_ReturnsFalse()
    {
        TwitchOutputMessageMatch? left = GetNullMatch();
        TwitchOutputMessageMatch right = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);

        Assert.False(condition: left == right, userMessage: "Null left operand should not equal non-null right via ==");
    }

    [Fact]
    public void OperatorEqual_RightNull_ReturnsFalse()
    {
        TwitchOutputMessageMatch left = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);
        TwitchOutputMessageMatch? right = GetNullMatch();

        Assert.False(condition: left == right, userMessage: "Non-null left operand should not equal null right via ==");
    }

    [Fact]
    public void OperatorEqual_EqualValues_ReturnsTrue()
    {
        TwitchOutputMessageMatch left = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);
        TwitchOutputMessageMatch right = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);

        Assert.True(condition: left == right, userMessage: "Matches with equal values should be equal via ==");
    }

    [Fact]
    public void OperatorEqual_UnequalValues_ReturnsFalse()
    {
        TwitchOutputMessageMatch left = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);
        TwitchOutputMessageMatch right = new(streamer: OTHER_STREAMER, message: TEST_MESSAGE);

        Assert.False(condition: left == right, userMessage: "Matches with different values should not be equal via ==");
    }

    [Fact]
    public void OperatorNotEqual_BothNull_ReturnsFalse()
    {
        TwitchOutputMessageMatch? left = GetNullMatch();
        TwitchOutputMessageMatch? right = GetNullMatch();

        Assert.False(condition: left != right, userMessage: "Two null values should not be unequal via !=");
    }

    [Fact]
    public void OperatorNotEqual_LeftNull_ReturnsTrue()
    {
        TwitchOutputMessageMatch? left = GetNullMatch();
        TwitchOutputMessageMatch right = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);

        Assert.True(
            condition: left != right,
            userMessage: "Null left operand should be unequal to non-null right via !="
        );
    }

    [Fact]
    public void OperatorNotEqual_RightNull_ReturnsTrue()
    {
        TwitchOutputMessageMatch left = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);
        TwitchOutputMessageMatch? right = GetNullMatch();

        Assert.True(
            condition: left != right,
            userMessage: "Non-null left operand should be unequal to null right via !="
        );
    }

    [Fact]
    public void OperatorNotEqual_EqualValues_ReturnsFalse()
    {
        TwitchOutputMessageMatch left = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);
        TwitchOutputMessageMatch right = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);

        Assert.False(condition: left != right, userMessage: "Matches with equal values should not be unequal via !=");
    }

    [Fact]
    public void OperatorNotEqual_UnequalValues_ReturnsTrue()
    {
        TwitchOutputMessageMatch left = new(streamer: TEST_STREAMER, message: TEST_MESSAGE);
        TwitchOutputMessageMatch right = new(streamer: OTHER_STREAMER, message: TEST_MESSAGE);

        Assert.True(condition: left != right, userMessage: "Matches with different values should be unequal via !=");
    }
}
