using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Actions.Services.LoggingExtensions;
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

    public async Task SendAsync(Streamer streamer, string message, CancellationToken cancellationToken)
    {
        this._logger.CustomMessageTriggeredSending(streamer: streamer, message: message);
        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, message: message, cancellationToken: cancellationToken);
        this._logger.CustomMessageTriggeredSent(streamer: streamer, message: message);
    }
}