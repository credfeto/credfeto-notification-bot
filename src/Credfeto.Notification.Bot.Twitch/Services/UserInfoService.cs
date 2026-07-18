using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Date.Interfaces;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Services.LoggingExtensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NonBlocking;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class UserInfoService : IUserInfoService
{
    private static readonly TimeSpan NegativeCacheTtl = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan PositiveCacheTtl = TimeSpan.FromMinutes(30);

    private readonly TwitchAPI _api;
    private readonly ICurrentTimeSource _currentTimeSource;

    private readonly ConcurrentDictionary<Viewer, CacheEntry> _cache;
    private readonly ILogger<UserInfoService> _logger;

    public UserInfoService(
        IOptions<TwitchBotOptions> options,
        ICurrentTimeSource currentTimeSource,
        ILogger<UserInfoService> logger
    )
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._currentTimeSource = currentTimeSource ?? throw new ArgumentNullException(nameof(currentTimeSource));
        TwitchBotOptions apiOptions = (options ?? throw new ArgumentNullException(nameof(options))).Value;

        this._api = apiOptions.ConfigureTwitchApi();

        this._cache = new();
    }

    public Task<TwitchUser?> GetUserAsync(in Streamer userName, in CancellationToken cancellationToken)
    {
        return this.GetUserAsync(userName.ToViewer(), cancellationToken: cancellationToken);
    }

    public async Task<TwitchUser?> GetUserAsync(Viewer userName, CancellationToken cancellationToken)
    {
        while (true)
        {
            DateTimeOffset now = this._currentTimeSource.UtcNow();
            CacheEntry entry = this._cache.GetOrAdd(
                key: userName,
                value: new CacheEntry(fetch: () => this.FetchUserAsync(userName), createdAt: now)
            );

            TwitchUser? user;

            try
            {
                user = await entry.GetValueAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch
            {
                this._cache.TryRemove(new(key: userName, value: entry));

                throw;
            }

            TimeSpan ttl = user is null ? NegativeCacheTtl : PositiveCacheTtl;

            if (now < entry.CreatedAt + ttl)
            {
                return user;
            }

            this._cache.TryRemove(new(key: userName, value: entry));
        }
    }

    private async Task<TwitchUser?> FetchUserAsync(Viewer userName)
    {
        try
        {
            this._logger.GettingUserInfo(userName);
            GetUsersResponse result = await this.GetUsersAsync(userName);

            return result.Users is [] ? null : ConvertUser(result.Users[0]);
        }
        catch (Exception exception)
        {
            this._logger.FailedToGetUserInformation(
                userName: userName,
                message: exception.Message,
                exception: exception
            );

            throw;
        }
    }

    private Task<GetUsersResponse> GetUsersAsync(in Viewer userName)
    {
        return this._api.Helix.Users.GetUsersAsync(logins: [userName.Value]);
    }

    private static TwitchUser ConvertUser(User user)
    {
        return new(
            Convert.ToInt32(value: user.Id, provider: CultureInfo.InvariantCulture),
            Viewer.FromString(user.Login),
            !string.IsNullOrWhiteSpace(user.BroadcasterType),
            user.CreatedAt.AsDateTimeOffset()
        );
    }

    private sealed class CacheEntry
    {
        private readonly Lazy<Task<TwitchUser?>> _fetch;

        [SuppressMessage(
            category: "Microsoft.VisualStudio.Threading",
            checkId: "VSTHRD011: Use AsyncLazy<T> instead",
            Justification = "Background worker service has no JoinableTaskFactory/UI thread to deadlock against; Lazy<Task<T>> coalesces concurrent fetches for the same cache key"
        )]
        public CacheEntry(Func<Task<TwitchUser?>> fetch, in DateTimeOffset createdAt)
        {
            this._fetch = new(fetch, mode: LazyThreadSafetyMode.ExecutionAndPublication);
            this.CreatedAt = createdAt;
        }

        public DateTimeOffset CreatedAt { get; }

        public Task<TwitchUser?> GetValueAsync(in CancellationToken cancellationToken)
        {
            return this._fetch.Value.WaitAsync(cancellationToken);
        }
    }
}
