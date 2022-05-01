namespace Credfeto.Notification.Bot.Twitch.Interfaces;

public interface ITwitchStreamSettings
{
    bool ChatWelcomesEnabled { get; }

    bool RaidWelcomesEnabled { get; }

    bool ThanksEnabled { get; }

    bool AnnounceMilestonesEnabled { get; }

    bool ShoutOutsEnabled { get; }

    bool OverrideShoutOuts(bool value);

    bool OverrideWelcomes(bool value);

    bool OverrideRaidWelcomes(bool value);

    bool OverrideThanks(bool value);

    bool OverrideMilestonesEnabled(bool value);
}