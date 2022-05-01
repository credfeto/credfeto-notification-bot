using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;

namespace Credfeto.Notification.Bot.Twitch.Services;

internal sealed class TwitchStreamSettingsOffline : TwitchStreamSettingsBase, ITwitchStreamSettings
{
    public TwitchStreamSettingsOffline(TwitchBotOptions options, in Streamer streamer)
        : base(options: options, streamer: streamer)
    {
        this.ChatWelcomesEnabled = this.ModChannel.Welcome.Enabled;
        this.RaidWelcomesEnabled = this.ModChannel.Raids.Enabled;
        this.ThanksEnabled = this.ModChannel.Thanks.Enabled;
        this.AnnounceMilestonesEnabled = this.ModChannel.MileStones.Enabled;
    }

    public bool ChatWelcomesEnabled { get; }

    public bool OverrideWelcomes(bool value)
    {
        return false;
    }

    public bool RaidWelcomesEnabled { get; }

    public bool OverrideRaidWelcomes(bool value)
    {
        return false;
    }

    public bool ThanksEnabled { get; }

    public bool OverrideThanks(bool value)
    {
        return false;
    }

    public bool AnnounceMilestonesEnabled { get; }

    public bool OverrideMilestonesEnabled(bool value)
    {
        return false;
    }
}