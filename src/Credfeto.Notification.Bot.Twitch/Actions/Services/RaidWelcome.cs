using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.StreamState;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class RaidWelcome : MessageSenderBase, IRaidWelcome
{
    private readonly ILogger<RaidWelcome> _logger;
    private readonly ITwitchChannelManager _twitchChannelManager;

    public RaidWelcome(ITwitchChannelManager twitchChannelManager, IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ILogger<RaidWelcome> logger)
        : base(twitchChatMessageChannel)
    {
        this._twitchChannelManager = twitchChannelManager ?? throw new ArgumentNullException(nameof(twitchChannelManager));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task IssueRaidWelcomeAsync(Streamer streamer, Viewer raider, CancellationToken cancellationToken)
    {
        ITwitchChannelState modChannel = this._twitchChannelManager.GetStreamer(streamer);

        if (!modChannel.Settings.RaidWelcomesEnabled)
        {
            return;
        }

        const string raidWelcome = @"
♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫
GlitchLit  GlitchLit  GlitchLit Welcome raiders! GlitchLit GlitchLit GlitchLit
♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫";

        await this.SendMessageAsync(streamer: streamer, message: raidWelcome, cancellationToken: cancellationToken);
        await this.SendMessageAsync(streamer: streamer, $"Thanks @{raider} for the raid", cancellationToken: cancellationToken);
        await this.SendMessageAsync(streamer: streamer, $"!so @{raider}", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{streamer}: {raider} is raiding!");
    }
}