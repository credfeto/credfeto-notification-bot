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
        this.AnnounceMilestonesEnabled = this.ModChannel.MileStones.Enabled;
        this.ShoutOutsEnabled = this.ModChannel.ShoutOuts.Enabled;

        this._logger.LogWarning($"Online status for {this._streamer}");
        this._logger.LogWarning($"Chat Welcomes: {this.ChatWelcomesEnabled}");
        this._logger.LogWarning($"Raid Welcomes: {this.RaidWelcomesEnabled}");
        this._logger.LogWarning($"Thanks: {this.RaidWelcomesEnabled}");
        this._logger.LogWarning($"Milestones: {this.AnnounceMilestonesEnabled}");
        this._logger.LogWarning($"Shout-Outs: {this.ShoutOutsEnabled}");
    }

    public bool ShoutOutsEnabled { get; private set; }

    public bool OverrideShoutOuts(bool value)
    {
        if (!this.CanOverrideShoutOuts)
        {
            return false;
        }

        if (this.ShoutOutsEnabled == value)
        {
            return true;
        }

        this.ShoutOutsEnabled = value;
        this._logger.LogWarning($"{this._streamer}: Shout-Outs have been {AsEnabled(value)}.");

        return true;
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

    public bool AnnounceMilestonesEnabled { get; private set; }

    public bool OverrideMilestonesEnabled(bool value)
    {
        if (!this.CanOverrideAnnounceMilestones)
        {
            return false;
        }

        if (this.AnnounceMilestonesEnabled == value)
        {
            return true;
        }

        this.AnnounceMilestonesEnabled = value;
        this._logger.LogWarning($"{this._streamer}: Milestone announcements have been {AsEnabled(value)}.");

        return true;
    }

    private static string AsEnabled(bool value)
    {
        return value
            ? "enabled"
            : "disabled";
    }
}