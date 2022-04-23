namespace Credfeto.Notification.Bot.Twitch.Interfaces;

public interface ITwitchStreamSettings
{
    bool WelcomesEnabled { get; }

    bool OverrideWelcomes(bool value);
}