using System;
using System.Diagnostics.CodeAnalysis;

namespace Credfeto.Notification.Dapper;

/// <summary>
///     Calculates delay retries.
/// </summary>
public static class RetryDelayCalculator
{
    private static readonly Random RandomNumberGenerator = new();

    /// <summary>
    ///     Calculate the retry delay based on the current number of attempts
    /// </summary>
    /// <param name="attempts">The number of attempts</param>
    /// <returns>The time to wait before the next attempt</returns>
    public static TimeSpan Calculate(int attempts)
    {
        // do a fast first retry, then exponential backoff
        return attempts <= 1
            ? TimeSpan.Zero
            : TimeSpan.FromSeconds(CalculateBackoff(attempts));
    }

    /// <summary>
    ///     Calculate the retry delay based on the current number of attempts
    /// </summary>
    /// <param name="attempts">The number of attempts</param>
    /// <param name="maxJitterSeconds">The maximum number of seconds for jitter.</param>
    /// <returns>The time to wait before the next attempt</returns>
    public static TimeSpan CalculateWithJitter(int attempts, int maxJitterSeconds)
    {
        // do a fast first retry, then exponential backoff
        return attempts <= 1
            ? TimeSpan.Zero
            : TimeSpan.FromSeconds(WithJitter(CalculateBackoff(attempts), maxSeconds: maxJitterSeconds));
    }

    private static double CalculateBackoff(int attempts)
    {
        return Math.Pow(x: 2, y: attempts);
    }

    private static double WithJitter(double delaySeconds, int maxSeconds)
    {
        double nonJitterPeriod = delaySeconds - maxSeconds;
        double jitterRange = maxSeconds * 2;

        if (nonJitterPeriod < 0)
        {
            jitterRange = delaySeconds;
            nonJitterPeriod = delaySeconds / 2;
        }

        double jitter = CalculateJitterSeconds(jitterRange);

        return nonJitterPeriod + jitter;
    }

    [SuppressMessage(category: "Microsoft.Security", checkId: "CA5394:Do not use insecure randomness", Justification = "Just a re-try delay")]
    private static double CalculateJitterSeconds(double jitterRange)
    {
        return jitterRange * RandomNumberGenerator.NextDouble();
    }
}