using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.StreamState;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class RaidWelcome : MessageSenderBase, IRaidWelcome
{
    private readonly ILogger<RaidWelcome> _logger;

    public RaidWelcome(IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ILogger<RaidWelcome> logger)
        : base(twitchChatMessageChannel)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task IssueRaidWelcomeAsync(string channel, string raider, CancellationToken cancellationToken)
    {
        const string raidWelcome = @"
♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫
GlitchLit  GlitchLit  GlitchLit Welcome raiders! GlitchLit GlitchLit GlitchLit
♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫♫";

        await this.SendMessageAsync(channel: channel, message: raidWelcome, cancellationToken: cancellationToken);
        await this.SendMessageAsync(channel: channel, $"Thanks @{raider} for the raid", cancellationToken: cancellationToken);
        await this.SendMessageAsync(channel: channel, $"Check out https://www.twitch.tv/{raider}", cancellationToken: cancellationToken);

        this._logger.LogInformation($"{channel}: {raider} is raiding!");
    }
}