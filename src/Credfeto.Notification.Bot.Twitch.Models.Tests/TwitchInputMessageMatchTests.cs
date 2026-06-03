using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Models.Tests;

public sealed class TwitchInputMessageMatchTests : TestBase
{
    private static readonly Streamer TEST_STREAMER = Streamer.FromString("teststreamer");
    private static readonly Streamer OTHER_STREAMER = Streamer.FromString("otherstreamer");
    private static readonly Viewer TEST_CHATTER = Viewer.FromString("testchatter");
    private static readonly Viewer OTHER_CHATTER = Viewer.FromString("otherchatter");
    private const string TEST_MESSAGE = "hello";
    private const string OTHER_MESSAGE = "world";
    private const string TEST_REGEX = "^hello.*$";

    private static TwitchInputMessageMatch? GetNullMatch() => null;

    [Fact]
    public void Constructor_WithExactMatchType_RegexIsNull()
    {
        TwitchInputMessageMatch match = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.Null(match.Regex);
    }

    [Fact]
    public void Constructor_WithStartsWithMatchType_RegexIsNull()
    {
        TwitchInputMessageMatch match = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.STARTS_WITH
        );

        Assert.Null(match.Regex);
    }

    [Fact]
    public void Constructor_WithEndsWithMatchType_RegexIsNull()
    {
        TwitchInputMessageMatch match = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.ENDS_WITH
        );

        Assert.Null(match.Regex);
    }

    [Fact]
    public void Constructor_WithContainsMatchType_RegexIsNull()
    {
        TwitchInputMessageMatch match = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.CONTAINS
        );

        Assert.Null(match.Regex);
    }

    [Fact]
    public void Constructor_WithRegexMatchType_RegexIsNotNull()
    {
        TwitchInputMessageMatch match = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_REGEX,
            matchType: TwitchMessageMatchType.REGEX
        );

        Assert.NotNull(match.Regex);
    }

    [Fact]
    public void Constructor_WithRegexMatchType_SecondCallWithSameExpression_ReturnsCachedRegex()
    {
        TwitchInputMessageMatch first = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_REGEX,
            matchType: TwitchMessageMatchType.REGEX
        );
        TwitchInputMessageMatch second = new(
            streamer: OTHER_STREAMER,
            chatter: OTHER_CHATTER,
            message: TEST_REGEX,
            matchType: TwitchMessageMatchType.REGEX
        );

        Assert.NotNull(first.Regex);
        Assert.NotNull(second.Regex);
        Assert.Same(expected: first.Regex, actual: second.Regex);
    }

    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        TwitchInputMessageMatch match = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.False(condition: match.Equals(GetNullMatch()), userMessage: "Equals should return false for null");
    }

    [Fact]
    public void Equals_WithSameReference_ReturnsTrue()
    {
        TwitchInputMessageMatch match = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.True(condition: match.Equals(match), userMessage: "Equals should return true for same reference");
    }

    [Fact]
    public void Equals_WithEqualValues_ReturnsTrue()
    {
        TwitchInputMessageMatch first = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );
        TwitchInputMessageMatch second = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.True(condition: first.Equals(second), userMessage: "Equals should return true for equal values");
    }

    [Fact]
    public void Equals_WithDifferentStreamer_ReturnsFalse()
    {
        TwitchInputMessageMatch first = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );
        TwitchInputMessageMatch second = new(
            streamer: OTHER_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.False(
            condition: first.Equals(second),
            userMessage: "Equals should return false for different streamers"
        );
    }

    [Fact]
    public void Equals_WithDifferentChatter_ReturnsFalse()
    {
        TwitchInputMessageMatch first = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );
        TwitchInputMessageMatch second = new(
            streamer: TEST_STREAMER,
            chatter: OTHER_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.False(condition: first.Equals(second), userMessage: "Equals should return false for different chatters");
    }

    [Fact]
    public void Equals_WithDifferentMessage_ReturnsFalse()
    {
        TwitchInputMessageMatch first = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );
        TwitchInputMessageMatch second = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: OTHER_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.False(condition: first.Equals(second), userMessage: "Equals should return false for different messages");
    }

    [Fact]
    public void Equals_WithDifferentMatchType_ReturnsFalse()
    {
        TwitchInputMessageMatch first = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );
        TwitchInputMessageMatch second = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.CONTAINS
        );

        Assert.False(
            condition: first.Equals(second),
            userMessage: "Equals should return false for different match types"
        );
    }

    [Fact]
    public void Equals_Object_WithNull_ReturnsFalse()
    {
        TwitchInputMessageMatch match = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );
        object? nullObject = GetNullMatch();

        Assert.False(condition: match.Equals(nullObject), userMessage: "Equals(object) should return false for null");
    }

    [Fact]
    public void Equals_Object_WithSameReference_ReturnsTrue()
    {
        TwitchInputMessageMatch match = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.True(
            condition: match.Equals((object)match),
            userMessage: "Equals(object) should return true for same reference"
        );
    }

    [Fact]
    public void Equals_Object_WithEqualObject_ReturnsTrue()
    {
        TwitchInputMessageMatch first = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );
        TwitchInputMessageMatch second = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.True(
            condition: first.Equals((object)second),
            userMessage: "Equals(object) should return true for equal objects"
        );
    }

    [Fact]
    public void Equals_Object_WithUnequalObject_ReturnsFalse()
    {
        TwitchInputMessageMatch first = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );
        TwitchInputMessageMatch second = new(
            streamer: OTHER_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.False(
            condition: first.Equals((object)second),
            userMessage: "Equals(object) should return false for unequal objects"
        );
    }

    [Fact]
    public void Equals_Object_WithDifferentType_ReturnsFalse()
    {
        TwitchInputMessageMatch match = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.False(
            condition: match.Equals("not a TwitchInputMessageMatch"),
            userMessage: "Equals(object) should return false for different types"
        );
    }

    [Fact]
    public void GetHashCode_IsConsistentForEqualValues()
    {
        TwitchInputMessageMatch first = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );
        TwitchInputMessageMatch second = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.Equal(expected: first.GetHashCode(), actual: second.GetHashCode());
    }

    [Fact]
    public void OperatorEqual_BothNull_ReturnsTrue()
    {
        TwitchInputMessageMatch? left = GetNullMatch();
        TwitchInputMessageMatch? right = GetNullMatch();

        Assert.True(condition: left == right, userMessage: "Two null values should be equal via ==");
    }

    [Fact]
    public void OperatorEqual_LeftNull_ReturnsFalse()
    {
        TwitchInputMessageMatch? left = GetNullMatch();
        TwitchInputMessageMatch right = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.False(condition: left == right, userMessage: "Null left operand should not equal non-null right via ==");
    }

    [Fact]
    public void OperatorEqual_RightNull_ReturnsFalse()
    {
        TwitchInputMessageMatch left = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );
        TwitchInputMessageMatch? right = GetNullMatch();

        Assert.False(condition: left == right, userMessage: "Non-null left operand should not equal null right via ==");
    }

    [Fact]
    public void OperatorEqual_EqualValues_ReturnsTrue()
    {
        TwitchInputMessageMatch left = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );
        TwitchInputMessageMatch right = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.True(condition: left == right, userMessage: "Matches with equal values should be equal via ==");
    }

    [Fact]
    public void OperatorEqual_UnequalValues_ReturnsFalse()
    {
        TwitchInputMessageMatch left = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );
        TwitchInputMessageMatch right = new(
            streamer: OTHER_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.False(condition: left == right, userMessage: "Matches with different values should not be equal via ==");
    }

    [Fact]
    public void OperatorNotEqual_BothNull_ReturnsFalse()
    {
        TwitchInputMessageMatch? left = GetNullMatch();
        TwitchInputMessageMatch? right = GetNullMatch();

        Assert.False(condition: left != right, userMessage: "Two null values should not be unequal via !=");
    }

    [Fact]
    public void OperatorNotEqual_LeftNull_ReturnsTrue()
    {
        TwitchInputMessageMatch? left = GetNullMatch();
        TwitchInputMessageMatch right = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.True(
            condition: left != right,
            userMessage: "Null left operand should be unequal to non-null right via !="
        );
    }

    [Fact]
    public void OperatorNotEqual_RightNull_ReturnsTrue()
    {
        TwitchInputMessageMatch left = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );
        TwitchInputMessageMatch? right = GetNullMatch();

        Assert.True(
            condition: left != right,
            userMessage: "Non-null left operand should be unequal to null right via !="
        );
    }

    [Fact]
    public void OperatorNotEqual_EqualValues_ReturnsFalse()
    {
        TwitchInputMessageMatch left = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );
        TwitchInputMessageMatch right = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.False(condition: left != right, userMessage: "Matches with equal values should not be unequal via !=");
    }

    [Fact]
    public void OperatorNotEqual_UnequalValues_ReturnsTrue()
    {
        TwitchInputMessageMatch left = new(
            streamer: TEST_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );
        TwitchInputMessageMatch right = new(
            streamer: OTHER_STREAMER,
            chatter: TEST_CHATTER,
            message: TEST_MESSAGE,
            matchType: TwitchMessageMatchType.EXACT
        );

        Assert.True(condition: left != right, userMessage: "Matches with different values should be unequal via !=");
    }
}
