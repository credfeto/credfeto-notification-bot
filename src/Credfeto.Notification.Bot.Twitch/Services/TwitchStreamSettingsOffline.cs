using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;

namespace Credfeto.Notification.Bot.Twitch.Services;

internal sealed class TwitchStreamSettingsOffline : TwitchStreamSettingsBase, ITwitchStreamSettings
{
    public TwitchStreamSettingsOffline(TwitchBotOptions options, in Streamer streamer)
        : base(options: options, streamer: streamer)
    {
        this.WelcomesEnabled = this.ModChannel.Welcome.Enabled;
    }

    public bool WelcomesEnabled { get; }

    public bool OverrideWelcomes(bool value)
    {
        return false;
    }
}