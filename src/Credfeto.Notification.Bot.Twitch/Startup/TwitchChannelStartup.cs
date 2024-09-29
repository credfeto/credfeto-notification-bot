using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Services.Startup.Interfaces;
using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Twitch.Startup;

public sealed class TwitchChannelStartup : IRunOnStartup
{
    private readonly IReadOnlyList<Streamer> _channels;
    private readonly ILogger<TwitchChannelStartup> _logger;
    private readonly IMediator _mediator;
    private readonly TwitchBotOptions _options;
    private readonly ITwitchChat _twitchChat;
    private readonly IUserInfoService _userInfoService;

    public TwitchChannelStartup(IOptions<TwitchBotOptions> options, IUserInfoService userInfoService, ITwitchChat twitchChat, IMediator mediator, ILogger<TwitchChannelStartup> logger)
    {
        this._userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        this._twitchChat = twitchChat ?? throw new ArgumentNullException(nameof(twitchChat));
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

        this._channels = new[]
            {
                this._options.Authentication.UserName
            }.Concat(this._options.ChatCommands.Select(channel => channel.Streamer))
             .Select(Streamer.FromString)
             .Distinct()
             .ToList();
    }

    public async ValueTask StartAsync(CancellationToken cancellationToken)
    {
        // Explicitly give some time for things to initialize
        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken: cancellationToken);

        // Explicitly join the bot's own channel
        this._twitchChat.JoinChat(Streamer.FromString(this._options.Authentication.UserName));

        foreach (Streamer streamer in this._channels)
        {
            this._logger.LogInformation($"Looking for channel {streamer}");
            TwitchUser? info = await this._userInfoService.GetUserAsync(userName: streamer, cancellationToken: cancellationToken);

            if (info != null)
            {
                this._logger.LogInformation($"Streamer: {streamer}");
                await this._mediator.Publish(new TwitchWatchChannel(info), cancellationToken: cancellationToken);
            }
        }
    }
}