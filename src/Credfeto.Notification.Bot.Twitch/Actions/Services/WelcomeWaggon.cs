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

public sealed class WelcomeWaggon : MessageSenderBase, IWelcomeWaggon
{
    private readonly ILogger<WelcomeWaggon> _logger;
    private readonly TwitchBotOptions _options;

    public WelcomeWaggon(IOptions<TwitchBotOptions> options, IMessageChannel<TwitchChatMessage> twitchChatMessageChannel, ILogger<WelcomeWaggon> logger)
        : base(twitchChatMessageChannel)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

        this._options = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task IssueWelcomeAsync(Streamer streamer, Viewer user, CancellationToken cancellationToken)
    {
        TwitchModChannel? modChannel = this._options.GetModChannel(streamer);

        if (modChannel?.Welcome.Enabled != true)
        {
            return;
        }

        await this.SendMessageAsync(streamer: streamer, $"Hi @{user}", cancellationToken: cancellationToken);
        this._logger.LogInformation($"{streamer}: Hi {user}!");
    }
}