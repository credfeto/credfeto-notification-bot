using System;
using System.Diagnostics;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Twitch.Services.LoggingExtensions;

internal static partial class UserInfoServiceLoggingExtensions
{
    [Conditional("DEBUG")]
    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "{userName}: Getting User information"
    )]
    public static partial void GettingUserInfo(
        this ILogger<UserInfoService> logger,
        Viewer userName
    );

    [LoggerMessage(
        EventId = 2,
        Level = LogLevel.Error,
        Message = "{userName}: Failed to get User information: {message}"
    )]
    public static partial void FailedToGetUserInformation(
        this ILogger<UserInfoService> logger,
        Viewer userName,
        string message,
        Exception exception
    );
}
