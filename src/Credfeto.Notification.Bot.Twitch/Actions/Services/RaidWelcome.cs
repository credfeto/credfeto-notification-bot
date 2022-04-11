using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.StreamState;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class RaidWelcome : MessageSenderBase, IRaidWelcome
{
    private readonly ILogger<RaidWelcome> _logger;
    private readonly TwitchBotOptions _options;

    public RaidWelcome(IOptions<TwitchBotOptions> options, IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ILogger<RaidWelcome> logger)
        : base(twitchChatMessageChannel)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task IssueRaidWelcomeAsync(Streamer streamer, Viewer raider, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(streamer);

        if (modChannel?.Raids.Enabled != true)
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