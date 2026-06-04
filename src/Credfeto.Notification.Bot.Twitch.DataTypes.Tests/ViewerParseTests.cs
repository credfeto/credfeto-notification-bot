using System;
using System.Globalization;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.DataTypes.Tests;

public sealed class ViewerParseTests : TestBase
{
    [Fact]
    public void Parse_StringWithFormatProvider_ReturnsViewer()
    {
        Viewer result = Viewer.Parse("testviewer", CultureInfo.InvariantCulture);

        Assert.Equal(expected: "testviewer", actual: result.Value);
    }

    [Fact]
    public void Parse_SpanWithFormatProvider_ReturnsViewer()
    {
        ReadOnlySpan<char> input = "testviewer".AsSpan();

        Viewer result = Viewer.Parse(input, CultureInfo.InvariantCulture);

        Assert.Equal(expected: "testviewer", actual: result.Value);
    }

    [Fact]
    public void TryParse_ValidString_ReturnsTrueAndViewer()
    {
        bool success = Viewer.TryParse("testviewer", CultureInfo.InvariantCulture, out Viewer result);

        Assert.True(success, userMessage: "TryParse should succeed with a valid string");
        Assert.Equal(expected: "testviewer", actual: result.Value);
    }

    [Fact]
    public void TryParse_NullString_ReturnsFalse()
    {
        bool success = Viewer.TryParse(null, CultureInfo.InvariantCulture, out Viewer result);

        Assert.False(success, userMessage: "TryParse should return false for null input");
        Assert.Equal(expected: default, actual: result);
    }

    [Fact]
    public void TryParse_Span_ReturnsTrueAndViewer()
    {
        ReadOnlySpan<char> input = "testviewer".AsSpan();

        bool success = Viewer.TryParse(input, CultureInfo.InvariantCulture, out Viewer result);

        Assert.True(success, userMessage: "TryParse span should always succeed");
        Assert.Equal(expected: "testviewer", actual: result.Value);
    }

    [Fact]
    public void ToString_WithFormatProvider_ReturnsValue()
    {
        Viewer viewer = Viewer.FromString("testviewer");

        string result = viewer.ToString(format: null, formatProvider: CultureInfo.InvariantCulture);

        Assert.Equal(expected: "testviewer", actual: result);
    }

    [Fact]
    public void ToString_ViaBoxedObject_ReturnsValue()
    {
        object boxed = Viewer.FromString("testviewer");

        string? result = boxed.ToString();

        Assert.Equal(expected: "testviewer", actual: result);
    }

    [Fact]
    public void TryFormat_SufficientBuffer_WritesValueAndReturnsTrue()
    {
        Viewer viewer = Viewer.FromString("testviewer");
        Span<char> buffer = stackalloc char[20];

        bool success = viewer.TryFormat(buffer, out int charsWritten, [], CultureInfo.InvariantCulture);

        Assert.True(success, userMessage: "TryFormat should succeed when buffer is large enough");
        Assert.Equal(expected: "testviewer".Length, actual: charsWritten);
    }

    [Fact]
    public void TryFormat_InsufficientBuffer_ReturnsFalse()
    {
        Viewer viewer = Viewer.FromString("testviewer");
        Span<char> buffer = stackalloc char[2];

        bool success = viewer.TryFormat(buffer, out int charsWritten, [], CultureInfo.InvariantCulture);

        Assert.False(success, userMessage: "TryFormat should fail when buffer is too small");
        Assert.Equal(expected: 0, actual: charsWritten);
    }
}
