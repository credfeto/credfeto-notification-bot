using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.StreamState;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Actions.Services;

public sealed class WelcomeWaggon : MessageSenderBase, IWelcomeWaggon
{
    private readonly ILogger<WelcomeWaggon> _logger;
    private readonly ITwitchChannelManager _twitchChannelManager;

    public WelcomeWaggon(ITwitchChannelManager twitchChannelManager, IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ILogger<WelcomeWaggon> logger)
        : base(twitchChatMessageChannel)
    {
        this._twitchChannelManager = twitchChannelManager ?? throw new ArgumentNullException(nameof(twitchChannelManager));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task IssueWelcomeAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        ITwitchChannelState channel = this._twitchChannelManager.GetStreamer(streamer);

        if (!channel.Settings.ChatWelcomesEnabled)
        {
            return;
        }

        await this.SendMessageAsync(streamer: streamer, $"Hi @{user}", cancellationToken: cancellationToken);
        this._logger.LogInformation($"{streamer}: Hi {user}!");
    }
}