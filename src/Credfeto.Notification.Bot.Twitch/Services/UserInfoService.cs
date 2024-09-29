using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Date.Interfaces;
using Credfeto.Notification.Bot.Twitch.Configuration;
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

    public UserInfoService(IOptions<TwitchBotOptions> options, ILogger<UserInfoService> logger)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        if (this._cache.TryGetValue(key: userName, out TwitchUser? user))
        {
            return user;
        }

        if (user != null)
        {
            this._cache.TryAdd(key: userName, value: user);

            return user;
        }

        try
        {
            this._logger.LogDebug($"Getting User information for {userName}");
            GetUsersResponse result = await this.GetUsersAsync(userName);

            if (result.Users.Length == 0)
            {
                this._cache.TryAdd(key: userName, value: null);

                return null;
            }

            user = ConvertUser(result.Users[0]);
            this._cache.TryAdd(key: userName, value: user);

            return user;
        }
        catch (Exception exception)
        {
            this._logger.LogError(new(exception.HResult), exception: exception, $"Failed to look up user information for {userName}: {exception.Message}");

            return null;
        }
    }

    private Task<GetUsersResponse> GetUsersAsync(in Viewer userName)
    {
        return this._api.Helix.Users.GetUsersAsync(logins: [userName.Value]);
    }

    private static TwitchUser ConvertUser(User user)
    {
        return new(UserName: Viewer.FromString(user.Login),
                   Id: Convert.ToInt32(value: user.Id, provider: CultureInfo.InvariantCulture),
                   IsStreamer: !string.IsNullOrWhiteSpace(user.BroadcasterType),
                   DateCreated: user.CreatedAt.AsDateTimeOffset());
    }
}