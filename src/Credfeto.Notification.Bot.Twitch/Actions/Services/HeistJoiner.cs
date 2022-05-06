using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.StreamState;
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

    public async Task JoinHeistAsync(Streamer streamer, CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{streamer}: Heist Starting!");
        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, message: "!heist all", cancellationToken: cancellationToken);
        this._logger.LogInformation($"{streamer}: Heist Starting! - Join Sent");
    }
}