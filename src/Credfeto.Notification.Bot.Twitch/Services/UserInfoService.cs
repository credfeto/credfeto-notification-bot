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
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class UserInfoService : IUserInfoService
{
    private readonly TwitchAPI _api;
    private readonly ILogger<UserInfoService> _logger;

    public UserInfoService(IOptions<TwitchBotOptions> options, ILogger<UserInfoService> logger)
    {
        this._logger = logger;
        TwitchBotOptions apiOptions = options.Value;

        this._api = apiOptions.ConfigureTwitchApi();
    }

    public Task<TwitchUser?> GetUserAsync(in Streamer userName, in CancellationToken cancellationToken)
    {
        return this.GetUserAsync(userName.ToViewer(), cancellationToken: cancellationToken);
    }

    public async Task<TwitchUser?> GetUserAsync(Viewer userName, CancellationToken cancellationToken)
    {
        try
        {
            this._logger.GettingUserInfo(userName);
            GetUsersResponse result = await this.GetUsersAsync(userName).WaitAsync(cancellationToken);

            return result.Users is [] ? null : ConvertUser(result.Users[0]);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
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
}
