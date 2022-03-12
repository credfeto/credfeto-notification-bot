using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Models;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class HeistJoiner : MessageSenderBase, IHeistJoiner
{
    private readonly ILogger<HeistJoiner> _logger;

    public HeistJoiner(IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ILogger<HeistJoiner> logger)
        : base(twitchChatMessageChannel)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task JoinHeistAsync(string channel, CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{channel}: Heist Starting!");
        await this.SendMessageAsync(channel: channel, message: "!heist all", cancellationToken: cancellationToken);
        this._logger.LogInformation($"{channel}: Heist Starting! - Join Sent");
    }
}