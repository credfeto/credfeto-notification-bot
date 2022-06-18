using System;
using System.Diagnostics.CodeAnalysis;

namespace Credfeto.Notification.Bot.Twitch.Services;

internal static class Jitter
{
    private static readonly Random RandomNumberGenerator = new();

    public static double WithJitter(double delaySeconds, int maxSeconds)
    {
        double minNonJitterPeriod = delaySeconds / 2.0d;
        double nonJitterPeriod = delaySeconds - maxSeconds / 2.0d;
        double jitterRange = maxSeconds * 2;

        if (nonJitterPeriod < minNonJitterPeriod)
        {
            jitterRange = delaySeconds;
            nonJitterPeriod = minNonJitterPeriod;
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