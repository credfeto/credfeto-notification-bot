using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.StreamState;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class CustomTriggeredMessageSender : MessageSenderBase, ICustomTriggeredMessageSender
{
    private readonly ILogger<CustomTriggeredMessageSender> _logger;

    public CustomTriggeredMessageSender(IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ILogger<CustomTriggeredMessageSender> logger)
        : base(twitchChatMessageChannel)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task JoinMarblesAsync(Streamer streamer, CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"{streamer}: Marbles Starting!");
        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, message: "!play", cancellationToken: cancellationToken);
        this._logger.LogInformation($"{streamer}: Marbles Starting! - Join Sent");
    }
}