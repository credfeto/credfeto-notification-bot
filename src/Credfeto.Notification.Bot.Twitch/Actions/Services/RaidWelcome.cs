using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.StreamState;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class RaidWelcome : MessageSenderBase, IRaidWelcome
{
    private readonly ILogger<RaidWelcome> _logger;
    private readonly TwitchBotOptions _options;
    private readonly string _raidWelcome;
    private readonly ITwitchChannelManager _twitchChannelManager;

    public RaidWelcome(IOptions<TwitchBotOptions> options,
                       ITwitchChannelManager twitchChannelManager,
                       IMessageChannel<TwitchChatMessage> twitchChatMessageChannel,
                       ILogger<RaidWelcome> logger)
        : base(twitchChatMessageChannel)
    {
        this._twitchChannelManager = twitchChannelManager ?? throw new ArgumentNullException(nameof(twitchChannelManager));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        this._raidWelcome = @"♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫" + Environment.NewLine + @"GlitchLit  GlitchLit  GlitchLit Welcome raiders! GlitchLit GlitchLit GlitchLit" +
                            Environment.NewLine + @"♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫";
    }

    public async Task IssueRaidWelcomeAsync(Streamer streamer, Viewer raider, CancellationToken cancellationToken)
    {
        ITwitchChannelState modChannel = this._twitchChannelManager.GetStreamer(streamer);

        if (!modChannel.Settings.RaidWelcomesEnabled)
        {
            return;
        }

        TwitchChannelRaids? raidSettings = this.GetStreamerSettings(streamer)
                                               ?.Raids;

        if (raidSettings?.Immediate != null)
        {
            foreach (string message in raidSettings.Immediate)
            {
                await this.SendImmediateMessageAsync(streamer: streamer, message: message, cancellationToken: cancellationToken);
            }
        }

        await this.SendImmediateMessageAsync(streamer: streamer, message: this._raidWelcome, cancellationToken: cancellationToken);
        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, $"Thanks @{raider} for the raid", cancellationToken: cancellationToken);
        await this.SendSlowMessageAsync(streamer: streamer, $"/shoutout @{raider}", cancellationToken: cancellationToken);

        if (raidSettings?.CalmDown != null)
        {
            foreach (string message in raidSettings.CalmDown)
            {
                await this.SendSlowMessageAsync(streamer: streamer, message: message, cancellationToken: cancellationToken);
            }
        }

        this._logger.LogInformation($"{streamer}: {raider} is raiding!");
    }

    private ValueTask SendSlowMessageAsync(in Streamer streamer, string message, in CancellationToken cancellationToken)
    {
        return this.SendMessageAsync(streamer: streamer, priority: MessagePriority.SLOW, message: message, cancellationToken: cancellationToken);
    }

    private ValueTask SendImmediateMessageAsync(in Streamer streamer, string message, in CancellationToken cancellationToken)
    {
        return this.SendMessageAsync(streamer: streamer, priority: MessagePriority.ASAP, message: message, cancellationToken: cancellationToken);
    }

    private TwitchModChannel? GetStreamerSettings(Streamer streamer)
    {
        return Array.Find(array: this._options.Channels, match: c => Streamer.FromString(c.ChannelName) == streamer);
    }
}