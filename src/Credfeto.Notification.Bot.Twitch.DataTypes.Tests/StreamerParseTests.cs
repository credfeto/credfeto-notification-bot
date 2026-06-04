using System;
using System.Globalization;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.DataTypes.Tests;

public sealed class StreamerParseTests : TestBase
{
    [Fact]
    public void Parse_StringWithFormatProvider_ReturnsStreamer()
    {
        Streamer result = Streamer.Parse("teststreamer", CultureInfo.InvariantCulture);

        Assert.Equal(expected: "teststreamer", actual: result.Value);
    }

    [Fact]
    public void Parse_SpanWithFormatProvider_ReturnsStreamer()
    {
        ReadOnlySpan<char> input = "teststreamer".AsSpan();

        Streamer result = Streamer.Parse(input, CultureInfo.InvariantCulture);

        Assert.Equal(expected: "teststreamer", actual: result.Value);
    }

    [Fact]
    public void TryParse_ValidString_ReturnsTrueAndStreamer()
    {
        bool success = Streamer.TryParse("teststreamer", CultureInfo.InvariantCulture, out Streamer result);

        Assert.True(success, userMessage: "TryParse should succeed with a valid string");
        Assert.Equal(expected: "teststreamer", actual: result.Value);
    }

    [Fact]
    public void TryParse_NullString_ReturnsFalse()
    {
        bool success = Streamer.TryParse(null, CultureInfo.InvariantCulture, out Streamer result);

        Assert.False(success, userMessage: "TryParse should return false for null input");
        Assert.Equal(expected: default, actual: result);
    }

    [Fact]
    public void TryParse_Span_ReturnsTrueAndStreamer()
    {
        ReadOnlySpan<char> input = "teststreamer".AsSpan();

        bool success = Streamer.TryParse(input, CultureInfo.InvariantCulture, out Streamer result);

        Assert.True(success, userMessage: "TryParse span should always succeed");
        Assert.Equal(expected: "teststreamer", actual: result.Value);
    }

    [Fact]
    public void ToString_WithFormatProvider_ReturnsValue()
    {
        Streamer streamer = Streamer.FromString("teststreamer");

        string result = streamer.ToString(format: null, formatProvider: CultureInfo.InvariantCulture);

        Assert.Equal(expected: "teststreamer", actual: result);
    }

    [Fact]
    public void ToString_ViaBoxedObject_ReturnsValue()
    {
        object boxed = Streamer.FromString("teststreamer");

        string? result = boxed.ToString();

        Assert.Equal(expected: "teststreamer", actual: result);
    }

    [Fact]
    public void TryFormat_SufficientBuffer_WritesValueAndReturnsTrue()
    {
        Streamer streamer = Streamer.FromString("teststreamer");
        Span<char> buffer = stackalloc char[20];

        bool success = streamer.TryFormat(buffer, out int charsWritten, [], CultureInfo.InvariantCulture);

        Assert.True(success, userMessage: "TryFormat should succeed when buffer is large enough");
        Assert.Equal(expected: "teststreamer".Length, actual: charsWritten);
    }

    [Fact]
    public void TryFormat_InsufficientBuffer_ReturnsFalse()
    {
        Streamer streamer = Streamer.FromString("teststreamer");
        Span<char> buffer = stackalloc char[2];

        bool success = streamer.TryFormat(buffer, out int charsWritten, [], CultureInfo.InvariantCulture);

        Assert.False(success, userMessage: "TryFormat should fail when buffer is too small");
        Assert.Equal(expected: 0, actual: charsWritten);
    }
}
