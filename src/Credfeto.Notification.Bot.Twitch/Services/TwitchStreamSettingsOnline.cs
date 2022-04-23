using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchStreamSettingsOnline : TwitchStreamSettingsBase, ITwitchStreamSettings
{
    public TwitchStreamSettingsOnline(TwitchBotOptions options, in Streamer streamer)
        : base(options: options, streamer: streamer)
    {
        this.WelcomesEnabled = this.ModChannel.Welcome.Enabled;
    }

    public bool WelcomesEnabled { get; private set; }

    public bool OverrideWelcomes(bool value)
    {
        if (this.CanOverrideWelcomes)
        {
            this.WelcomesEnabled = value;

            return false;
        }

        return true;
    }
}