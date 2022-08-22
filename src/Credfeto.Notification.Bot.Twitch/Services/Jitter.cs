using System;
using System.Security.Cryptography;

namespace Credfeto.Notification.Bot.Twitch.Services;

internal static class Jitter
{
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

    private static double CalculateJitterSeconds(double jitterRange)
    {
        return jitterRange * GetRandom();
    }

    private static double GetRandom()
    {
        using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
        {
            Span<byte> rnd = stackalloc byte[sizeof(uint)];
            randomNumberGenerator.GetBytes(rnd);
            uint random = BitConverter.ToUInt32(value: rnd);

            return random / (double)uint.MaxValue;
        }
    }
}