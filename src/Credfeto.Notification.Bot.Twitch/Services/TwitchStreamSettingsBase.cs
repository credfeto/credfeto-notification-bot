using System;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Extensions;

namespace Credfeto.Notification.Bot.Twitch.Services;

public abstract class TwitchStreamSettingsBase
{
    protected TwitchStreamSettingsBase(TwitchBotOptions options, in Streamer streamer)
    {
        this.ModChannel = (options ?? throw new ArgumentNullException(nameof(options))).GetModChannel(streamer) ?? throw new ArgumentNullException(nameof(streamer));
    }

    protected TwitchModChannel ModChannel { get; }

    protected bool CanOverrideChatWelcomes => !this.ModChannel.Welcome.Enabled;

    protected bool CanOverrideRaidWelcomes => !this.ModChannel.Raids.Enabled;

    protected bool CanOverrideThanks => !this.ModChannel.Thanks.Enabled;
}