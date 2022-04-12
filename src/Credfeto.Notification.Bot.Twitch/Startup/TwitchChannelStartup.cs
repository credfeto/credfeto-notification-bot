using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Twitch.Startup;

public sealed class TwitchChannelStartup : IRunOnStartup
{
    private readonly IReadOnlyList<Streamer> _channels;
    private readonly ILogger<TwitchChannelStartup> _logger;
    private readonly IMediator _mediator;
    private readonly TwitchBotOptions _options;
    private readonly ITwitchChannelManager _twitchChannelManager;
    private readonly IUserInfoService _userInfoService;

    public TwitchChannelStartup(IOptions<TwitchBotOptions> options,
                                IUserInfoService userInfoService,
                                ITwitchChannelManager twitchChannelManager,
                                IMediator mediator,
                                ILogger<TwitchChannelStartup> logger)
    {
        this._userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        this._twitchChannelManager = twitchChannelManager ?? throw new ArgumentNullException(nameof(twitchChannelManager));
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

        this._channels = new[]
                         {
                             this._options.Authentication.UserName
                         }.Concat(this._options.Channels.Select(channel => channel.ChannelName))
                          .Concat(this._options.Heists)
                          .Select(Streamer.FromString)
                          .Distinct()
                          .ToList();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (Streamer streamer in this._channels)
        {
            this._logger.LogInformation($"Looking for channel {streamer}");
            TwitchUser? info = await this._userInfoService.GetUserAsync(streamer);

            if (info != null)
            {
                ITwitchChannelState channel = this._twitchChannelManager.GetChannel(streamer);
                this._logger.LogInformation($"Streamer: {channel.Streamer}");
                await this._mediator.Publish(new TwitchChannelStartupEvent(info), cancellationToken: cancellationToken);
            }
        }
    }
}