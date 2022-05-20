using System;
using System.Diagnostics.CodeAnalysis;

namespace Credfeto.Notification.Bot.Database.Dapper;

public static class RetryDelayCalculator
{
    private static readonly Random RandomNumberGenerator = new();

    public static TimeSpan Calculate(int attempts)
    {
        // do a fast first retry, then exponential backoff
        return attempts <= 1
            ? TimeSpan.Zero
            : TimeSpan.FromSeconds(CalculateBackoff(attempts));
    }

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