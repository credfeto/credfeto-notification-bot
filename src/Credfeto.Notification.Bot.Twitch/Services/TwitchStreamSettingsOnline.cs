using System;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Services;

internal sealed class TwitchStreamSettingsOnline : TwitchStreamSettingsBase, ITwitchStreamSettings
{
    private readonly ILogger _logger;
    private readonly Streamer _streamer;

    public TwitchStreamSettingsOnline(TwitchBotOptions options, in Streamer streamer, ILogger logger)
        : base(options: options, streamer: streamer)
    {
        this._streamer = streamer;
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.ChatWelcomesEnabled = this.ModChannel.Welcome.Enabled;
        this.RaidWelcomesEnabled = this.ModChannel.Raids.Enabled;
        this.ThanksEnabled = this.ModChannel.Thanks.Enabled;
    }

    public bool ChatWelcomesEnabled { get; private set; }

    public bool OverrideWelcomes(bool value)
    {
        if (!this.CanOverrideChatWelcomes)
        {
            return false;
        }

        if (this.ChatWelcomesEnabled == value)
        {
            return true;
        }

        this.ChatWelcomesEnabled = value;
        this._logger.LogWarning($"{this._streamer}: Regular chatter welcomes have been {AsEnabled(value)}.");

        return true;
    }

    public bool RaidWelcomesEnabled { get; private set; }

    public bool OverrideRaidWelcomes(bool value)
    {
        if (!this.CanOverrideRaidWelcomes)
        {
            return false;
        }

        if (this.RaidWelcomesEnabled == value)
        {
            return true;
        }

        this.RaidWelcomesEnabled = value;
        this._logger.LogWarning($"{this._streamer}: Raid welcomes have been {AsEnabled(value)}.");

        return true;
    }

    public bool ThanksEnabled { get; private set; }

    public bool OverrideThanks(bool value)
    {
        if (!this.CanOverrideThanks)
        {
            return false;
        }

        if (this.ThanksEnabled == value)
        {
            return true;
        }

        this.ThanksEnabled = value;
        this._logger.LogWarning($"{this._streamer}: Thanks have been {AsEnabled(value)}.");

        return true;
    }

    private static string AsEnabled(bool value)
    {
        return value
            ? "enabled"
            : "disabled";
    }
}