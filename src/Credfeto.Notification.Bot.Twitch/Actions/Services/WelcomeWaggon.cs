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
    private readonly ITwitchChatMessageGenerator _twitchChatMessageGenerator;

    public WelcomeWaggon(ITwitchChannelManager twitchChannelManager,
                         IMessageChannel<TwitchChatMessage> twitchChatMessageChannel,
                         ITwitchChatMessageGenerator twitchChatMessageGenerator,
                         ILogger<WelcomeWaggon> logger)
        : base(twitchChatMessageChannel)
    {
        this._twitchChannelManager = twitchChannelManager ?? throw new ArgumentNullException(nameof(twitchChannelManager));
        this._twitchChatMessageGenerator = twitchChatMessageGenerator ?? throw new ArgumentNullException(nameof(twitchChatMessageGenerator));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task IssueWelcomeAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        ITwitchChannelState channel = this._twitchChannelManager.GetStreamer(streamer);

        if (!channel.Settings.ChatWelcomesEnabled)
        {
            return;
        }

        string message = this._twitchChatMessageGenerator.WelcomeMessage(user);
        await this.SendMessageAsync(streamer: streamer, priority: MessagePriority.NATURAL, message: message, cancellationToken: cancellationToken);
        this._logger.LogInformation($"{streamer}: Hi {user}!");
    }
}