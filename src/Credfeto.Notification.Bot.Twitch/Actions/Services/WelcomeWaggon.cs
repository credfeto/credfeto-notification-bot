using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Models;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class WelcomeWaggon : MessageSenderBase, IWelcomeWaggon
{
    private readonly ILogger<WelcomeWaggon> _logger;

    public WelcomeWaggon(IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ILogger<WelcomeWaggon> logger)
        : base(twitchChatMessageChannel)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task IssueWelcome(string channel, string user, CancellationToken cancellationToken)
    {
        await this.SendMessageAsync(channel: channel, $"Hi @{user}", cancellationToken: cancellationToken);
        this._logger.LogInformation($"{channel}: Hi {user}!");
    }
}