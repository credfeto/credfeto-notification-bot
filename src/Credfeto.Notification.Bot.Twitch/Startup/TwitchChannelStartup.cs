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
    public static readonly TimeSpan DefaultStartupDelay = TimeSpan.FromSeconds(3);

    private const int MaxAttempts = 3;

    private static readonly ResiliencePropertyKey<Streamer> StreamerContextKey = new("streamer");

    private readonly IReadOnlyList<Streamer> _channels;
    private readonly ILogger<TwitchChannelStartup> _logger;
    private readonly IMediator _mediator;
    private readonly TwitchBotOptions _options;
    private readonly ResiliencePipeline<TwitchUser?> _retryPipeline;
    private readonly TimeSpan _startupDelay;
    private readonly ITwitchChat _twitchChat;
    private readonly IUserInfoService _userInfoService;

    public TwitchChannelStartup(
        IOptions<TwitchBotOptions> options,
        IUserInfoService userInfoService,
        ITwitchChat twitchChat,
        IMediator mediator,
        in TimeSpan retryDelay,
        in TimeSpan startupDelay,
        ILogger<TwitchChannelStartup> logger
    )
    {
        this._userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        this._twitchChat = twitchChat ?? throw new ArgumentNullException(nameof(twitchChat));
        this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        this._startupDelay = startupDelay;

        this._channels =
        [
            .. new[] { this._options.Authentication.Chat.UserName }
                .Concat(this._options.ChatCommands.Select(channel => channel.Streamer))
                .Select(Streamer.FromString)
                .Distinct(),
        ];

        this._retryPipeline = new ResiliencePipelineBuilder<TwitchUser?>()
            .AddRetry(
                new RetryStrategyOptions<TwitchUser?>
                {
                    MaxRetryAttempts = MaxAttempts - 1,
                    Delay = retryDelay,
                    BackoffType = DelayBackoffType.Constant,
                    ShouldHandle = new PredicateBuilder<TwitchUser?>().Handle<Exception>(exception =>
                        exception is not OperationCanceledException
                    ),
                    OnRetry = this.OnRetryAsync,
                }
            )
            .Build();
    }

    private ValueTask OnRetryAsync(OnRetryArguments<TwitchUser?> arguments)
    {
        if (arguments.Outcome.Exception is { } exception)
        {
            // Polly v8 attempt numbers are zero-based; the log reports the one-based retry number.
            this._logger.RetryingChannelLookup(
                streamer: arguments.Context.Properties.GetValue(key: StreamerContextKey, defaultValue: default),
                attempt: arguments.AttemptNumber + 1,
                message: exception.Message,
                exception: exception
            );
        }

        return ValueTask.CompletedTask;
    }

    public async ValueTask StartAsync(CancellationToken cancellationToken)
    {
        // Explicitly give some time for things to initialize
        await Task.Delay(delay: this._startupDelay, cancellationToken: cancellationToken);

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
        ResilienceContext context = ResilienceContextPool.Shared.Get(cancellationToken);
        context.Properties.Set(key: StreamerContextKey, value: streamer);

        try
        {
            return await this._retryPipeline.ExecuteAsync(
                callback: static (resilienceContext, state) =>
                    new ValueTask<TwitchUser?>(
                        state.UserInfoService.GetUserAsync(
                            userName: state.Streamer,
                            cancellationToken: resilienceContext.CancellationToken
                        )
                    ),
                context: context,
                state: (UserInfoService: this._userInfoService, Streamer: streamer)
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
        finally
        {
            ResilienceContextPool.Shared.Return(context);
        }
    }
}
