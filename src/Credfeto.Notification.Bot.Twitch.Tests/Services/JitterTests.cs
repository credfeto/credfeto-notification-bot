using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Services;

public sealed class JitterTests : TestBase
{
    [Theory]
    [InlineData(30.0, 2)]
    [InlineData(15.0, 3)]
    [InlineData(60.0, 5)]
    public void WithJitterShouldReturnPositiveValue(double delaySeconds, int maxSeconds)
    {
        double result = Jitter.WithJitter(delaySeconds: delaySeconds, maxSeconds: maxSeconds);

        Assert.True(condition: result > 0, userMessage: "Jitter result should be positive");
    }

    [Theory]
    [InlineData(30.0, 2)]
    [InlineData(15.0, 3)]
    [InlineData(60.0, 5)]
    public void WithJitterShouldReturnValueWithinExpectedRange(double delaySeconds, int maxSeconds)
    {
        double result = Jitter.WithJitter(delaySeconds: delaySeconds, maxSeconds: maxSeconds);

        double upperBound = delaySeconds + maxSeconds * 2.0;
        Assert.True(condition: result <= upperBound, userMessage: "Jitter result should not exceed delay + 2*max");
    }

    [Fact]
    public void WithJitterWhenMaxExceedsDelayUsesHalfDelayAsBase()
    {
        const double delaySeconds = 2.0;
        const int maxSeconds = 10;

        double result = Jitter.WithJitter(delaySeconds: delaySeconds, maxSeconds: maxSeconds);

        Assert.True(
            condition: result >= delaySeconds / 2.0,
            userMessage: "Result should be at least half delay when maxSeconds exceeds delay"
        );
        Assert.True(
            condition: result <= delaySeconds * 2.0,
            userMessage: "Result should not exceed double the delay when branch is taken"
        );
    }
}
