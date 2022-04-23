using System;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class TwitchStreamSettingsOnline : TwitchStreamSettingsBase, ITwitchStreamSettings
{
    private readonly ILogger _logger;
    private readonly Streamer _streamer;

    public TwitchStreamSettingsOnline(TwitchBotOptions options, in Streamer streamer, ILogger logger)
        : base(options: options, streamer: streamer)
    {
        this._streamer = streamer;
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.WelcomesEnabled = this.ModChannel.Welcome.Enabled;
    }

    public bool WelcomesEnabled { get; private set; }

    public bool OverrideWelcomes(bool value)
    {
        if (this.CanOverrideWelcomes)
        {
            if (this.WelcomesEnabled != value)
            {
                this.WelcomesEnabled = value;
                this._logger.LogWarning($"{this._streamer}: Regular chatter welcomes have been {AsEnabled(value)}.");
            }

            return true;
        }

        return false;
    }

    private static string AsEnabled(bool value)
    {
        return value
            ? "enabled"
            : "disabled";
    }
}