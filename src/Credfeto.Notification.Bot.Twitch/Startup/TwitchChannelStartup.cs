using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Models;
using Credfeto.Notification.Bot.Twitch.Services.LoggingExtensions;
using Credfeto.Services.Startup.Interfaces;
using Mediator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Credfeto.Notification.Bot.Twitch.Startup;

public sealed class TwitchChannelStartup : IRunOnStartup
{
    public static readonly TimeSpan DefaultRetryDelay = TimeSpan.FromSeconds(5);

    private const int MaxAttempts = 3;
    private const string StreamerContextKey = "streamer";

    private readonly IReadOnlyList<Streamer> _channels;
    private readonly ILogger<TwitchChannelStartup> _logger;
    private readonly IMediator _mediator;
    private readonly TwitchBotOptions _options;
    private readonly AsyncRetryPolicy<TwitchUser?> _retryPolicy;
    private readonly ITwitchChat _twitchChat;
    private readonly IUserInfoService _userInfoService;

    public TwitchChannelStartup(
        IOptions<TwitchBotOptions> options,
        IUserInfoService userInfoService,
        ITwitchChat twitchChat,
        IMediator mediator,
        TimeSpan retryDelay,
        ILogger<TwitchChannelStartup> logger
    )
    {
        this._userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        this._twitchChat = twitchChat ?? throw new ArgumentNullException(nameof(twitchChat));
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;

        this._channels =
        [
            .. new[] { this._options.Authentication.Chat.UserName }
                .Concat(this._options.ChatCommands.Select(channel => channel.Streamer))
                .Select(Streamer.FromString)
                .Distinct(),
        ];

        this._retryPolicy = Policy<TwitchUser?>
            .Handle<Exception>(exception => exception is not OperationCanceledException)
            .WaitAndRetryAsync(
                retryCount: MaxAttempts - 1,
                sleepDurationProvider: _ => retryDelay,
                onRetry: (outcome, _, retryAttempt, context) =>
                {
                    Streamer streamer = (Streamer)context[StreamerContextKey];

                    this._logger.RetryingChannelLookup(
                        streamer: streamer,
                        attempt: retryAttempt,
                        message: outcome.Exception.Message,
                        exception: outcome.Exception
                    );
                }
            );
    }

    public async ValueTask StartAsync(CancellationToken cancellationToken)
    {
        // Explicitly give some time for things to initialize
        await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken: cancellationToken);

        // Explicitly join the bot's own channel
        await this._twitchChat.JoinChatAsync(Streamer.FromString(this._options.Authentication.Chat.UserName));

        foreach (Streamer streamer in this._channels)
        {
            this._logger.LookingForChannel(streamer);
            TwitchUser? info = await this.GetUserWithRetryAsync(
                streamer: streamer,
                cancellationToken: cancellationToken
            );

            if (info != null)
            {
                this._logger.FoundChannel(
                    streamer: streamer,
                    userId: info.Id,
                    isStreamer: info.IsStreamer,
                    dateCreated: info.DateCreated
                );
                await this._mediator.Publish(new TwitchWatchChannel(info), cancellationToken: cancellationToken);
            }
        }
    }

    private async ValueTask<TwitchUser?> GetUserWithRetryAsync(Streamer streamer, CancellationToken cancellationToken)
    {
        Context context = new() { [StreamerContextKey] = streamer };

        try
        {
            return await this._retryPolicy.ExecuteAsync(
                action: (_, ct) => this._userInfoService.GetUserAsync(userName: streamer, cancellationToken: ct),
                context: context,
                cancellationToken: cancellationToken
            );
        }
        catch (Exception exception)
            when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            this._logger.GivingUpOnChannelLookup(
                streamer: streamer,
                attempts: MaxAttempts,
                message: exception.Message,
                exception: exception
            );

            return null;
        }
    }
}
