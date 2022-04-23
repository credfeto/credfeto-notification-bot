namespace Credfeto.Notification.Bot.Twitch.Interfaces;

public interface ITwitchStreamSettings
{
    bool ChatWelcomesEnabled { get; }

    bool RaidWelcomesEnabled { get; }

    bool ThanksEnabled { get; }

    bool OverrideWelcomes(bool value);

    bool OverrideRaidWelcomes(bool value);

    bool OverrideThanks(bool value);
}