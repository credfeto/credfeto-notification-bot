namespace Credfeto.Notification.Bot.Shared;

public interface IRandomNumberGenerator
{
    int Next(int maxValue);
}