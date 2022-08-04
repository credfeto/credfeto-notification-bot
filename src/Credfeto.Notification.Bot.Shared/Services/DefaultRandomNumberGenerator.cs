using System.Security.Cryptography;

namespace Credfeto.Notification.Bot.Shared.Services;

public sealed class DefaultRandomNumberGenerator : IRandomNumberGenerator
{
    public int Next(int maxValue)
    {
        return RandomNumberGenerator.GetInt32(fromInclusive: 0, toExclusive: maxValue);
    }
}