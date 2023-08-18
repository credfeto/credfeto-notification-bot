using System;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Credfeto.Notification.Bot.Twitch.Data.Interfaces;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Extensions;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NonBlocking;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class UserInfoService : IUserInfoService
{
    private readonly TwitchAPI _api;

    private readonly ConcurrentDictionary<Viewer, TwitchUser?> _cache;
    private readonly ILogger<UserInfoService> _logger;
    private readonly ITwitchStreamerDataManager _twitchStreamerDataManager;
    private readonly ITwitchViewerDataManager _twitchViewerDataManager;

    public UserInfoService(IOptions<TwitchBotOptions> options, ITwitchStreamerDataManager twitchStreamerDataManager, ITwitchViewerDataManager twitchViewerDataManager, ILogger<UserInfoService> logger)
    {
        this._twitchStreamerDataManager = twitchStreamerDataManager ?? throw new ArgumentNullException(nameof(twitchStreamerDataManager));
        this._twitchViewerDataManager = twitchViewerDataManager ?? throw new ArgumentNullException(nameof(twitchViewerDataManager));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        TwitchBotOptions apiOptions = (options ?? throw new ArgumentNullException(nameof(options))).Value;

        this._api = apiOptions.ConfigureTwitchApi();

        this._cache = new();
    }

    public Task<TwitchUser?> GetUserAsync(in Streamer userName, in CancellationToken cancellationToken)
    {
        return this.GetUserAsync(userName.ToViewer(), cancellationToken);
    }

    public async Task<TwitchUser?> GetUserAsync(Viewer userName, CancellationToken cancellationToken)
    {
        if (this._cache.TryGetValue(key: userName, out TwitchUser? user))
        {
            return user;
        }

        user = await this.GetUserFromDatabaseAsync(userName, cancellationToken);

        if (user != null)
        {
            this._cache.TryAdd(key: userName, value: user);

            return user;
        }

        try
        {
            this._logger.LogDebug($"Getting User information for {userName}");
            GetUsersResponse result = await this._api.Helix.Users.GetUsersAsync(logins: new() { userName.Value });

            if (result.Users.Length == 0)
            {
                this._cache.TryAdd(key: userName, value: null);

                return null;
            }

            user = Convert(result.Users[0]);
            this._cache.TryAdd(key: userName, value: user);

            if (user.IsStreamer)
            {
                await this._twitchStreamerDataManager.AddStreamerAsync(user.UserName.ToStreamer(), streamerId: user.Id, startedStreaming: user.DateCreated, cancellationToken);
            }
            else
            {
                await this._twitchViewerDataManager.AddViewerAsync(viewerName: user.UserName, viewerId: user.Id, dateCreated: user.DateCreated, cancellationToken);
            }

            return user;
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"Failed to look up user information for {userName}: {exception.Message}");

            return null;
        }
    }

    private async Task<TwitchUser?> GetUserFromDatabaseAsync(Viewer userName, CancellationToken cancellationToken)
    {
        return await this._twitchStreamerDataManager.GetByUserNameAsync(userName: userName, cancellationToken: cancellationToken) ??
               await this._twitchViewerDataManager.GetByUserNameAsync(userName: userName, cancellationToken: cancellationToken);
    }

    private static TwitchUser Convert(User user)
    {
        return new(userName: Viewer.FromString(user.Login), id: user.Id, isStreamer: !string.IsNullOrWhiteSpace(user.BroadcasterType), dateCreated: user.CreatedAt);
    }
}