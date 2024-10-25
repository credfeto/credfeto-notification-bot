using System;
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

        try
        {
            this._logger.GettingUserInfo(userName);
            GetUsersResponse result = await this.GetUsersAsync(userName);

            return this.AddUserToCache(userName: userName,
                                       result.Users is []
                                           ? null
                                           : ConvertUser(result.Users[0]));
        }
        catch (Exception exception)
        {
            this._logger.FailedToGetUserInformation(userName: userName, message: exception.Message, exception: exception);

            return null;
        }
    }

    private TwitchUser? AddUserToCache(in Viewer userName, TwitchUser? user)
    {
        this._cache.TryAdd(key: userName, value: user);

        return user;
    }

    private Task<GetUsersResponse> GetUsersAsync(in Viewer userName)
    {
        return this._api.Helix.Users.GetUsersAsync(logins: [userName.Value]);
    }

    private static TwitchUser ConvertUser(User user)
    {
        return new(Convert.ToInt32(value: user.Id, provider: CultureInfo.InvariantCulture),
                   Viewer.FromString(user.Login),
                   !string.IsNullOrWhiteSpace(user.BroadcasterType),
                   user.CreatedAt.AsDateTimeOffset());
    }
}