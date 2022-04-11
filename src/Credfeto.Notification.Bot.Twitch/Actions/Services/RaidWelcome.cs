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

    public async Task IssueRaidWelcomeAsync(Channel channel, User raider, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(channel);

        if (modChannel?.Raids.Enabled != true)
        {
            return;
        }

        const string raidWelcome = @"
♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫
GlitchLit  GlitchLit  GlitchLit Welcome raiders! GlitchLit GlitchLit GlitchLit
♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫";

        await this.SendMessageAsync(channel: channel, message: raidWelcome, cancellationToken: cancellationToken);
        await this.SendMessageAsync(channel: channel, $"Thanks @{raider} for the raid", cancellationToken: cancellationToken);
        await this.SendMessageAsync(channel: channel, $"!so @{raider}", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: {raider} is raiding!");
    }
}