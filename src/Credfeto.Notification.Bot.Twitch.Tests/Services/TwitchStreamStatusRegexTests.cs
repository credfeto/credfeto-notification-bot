using System.Text.RegularExpressions;
using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Services;

public sealed class TwitchStreamStatusRegexTests : TestBase
{
    [Fact]
    public void NoChannelWithNameShouldMatchValidErrorMessage()
    {
        const string errorMessage = "No channel with the name \"teststreamer\" could be found.";

        Match match = TwitchStreamStatusRegex.NoChannelWithName().Match(errorMessage);

        Assert.True(condition: match.Success, userMessage: "Should match valid error message");
        Assert.Equal(expected: "teststreamer", actual: match.Groups["streamer"].Value);
    }

    [Theory]
    [InlineData("Some other error message")]
    [InlineData("No channel found")]
    [InlineData("No channel with the name \"\" could be found.")]
    [InlineData("")]
    public void NoChannelWithNameShouldNotMatchInvalidMessages(string message)
    {
        Match match = TwitchStreamStatusRegex.NoChannelWithName().Match(message);

        Assert.False(condition: match.Success, userMessage: $"Should not match invalid message: {message}");
    }

    [Fact]
    public void NoChannelWithNameShouldCaptureStreamerWithUnderscoreAndDigits()
    {
        const string errorMessage = "No channel with the name \"cool_streamer_123\" could be found.";

        Match match = TwitchStreamStatusRegex.NoChannelWithName().Match(errorMessage);

        Assert.True(condition: match.Success, userMessage: "Should match streamer name with underscores and digits");
        Assert.Equal(expected: "cool_streamer_123", actual: match.Groups["streamer"].Value);
    }
}
