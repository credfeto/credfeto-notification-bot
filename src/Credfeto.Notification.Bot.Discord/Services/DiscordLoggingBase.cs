using System;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Discord.Services;

/// <summary>
///     Base class for discord bot
/// </summary>
public abstract class DiscordLoggingBase
{
    private readonly ILogger _logger;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="logger">The logger</param>
    protected DiscordLoggingBase(ILogger logger)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    ///     Logs the message
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    protected Task LogAsync(LogMessage arg)
    {
        return arg.Severity switch
        {
            LogSeverity.Debug => this.LogDebugAsync(arg),
            LogSeverity.Verbose => this.LogInformationAsync(arg),
            LogSeverity.Info => this.LogInformationAsync(arg),
            LogSeverity.Warning => this.LogWarningAsync(arg),
            LogSeverity.Error => this.LogErrorAsync(arg),
            LogSeverity.Critical => this.LogCriticalAsync(arg),
            _ => this.LogDebugAsync(arg)
        };
    }

    private Task LogDebugAsync(LogMessage arg)
    {
        if (arg.Exception != null)
        {
            this._logger.LogDebug(new(arg.Exception.HResult), exception: arg.Exception, message: arg.Message);
        }
        else
        {
            this._logger.LogDebug(arg.Message);
        }

        return Task.CompletedTask;
    }

    private Task LogInformationAsync(LogMessage arg)
    {
        if (arg.Exception != null)
        {
            this._logger.LogInformation(new(arg.Exception.HResult), exception: arg.Exception, message: arg.Message);
        }
        else
        {
            this._logger.LogInformation(arg.Message);
        }

        return Task.CompletedTask;
    }

    private Task LogWarningAsync(LogMessage arg)
    {
        if (arg.Exception != null)
        {
            this._logger.LogWarning(new(arg.Exception.HResult), exception: arg.Exception, message: arg.Message);
        }
        else
        {
            this._logger.LogWarning(arg.Message);
        }

        return Task.CompletedTask;
    }

    private Task LogErrorAsync(LogMessage arg)
    {
        if (arg.Exception != null)
        {
            this._logger.LogError(new(arg.Exception.HResult), exception: arg.Exception, message: arg.Message);
        }
        else
        {
            this._logger.LogError(arg.Message);
        }

        return Task.CompletedTask;
    }

    private Task LogCriticalAsync(LogMessage arg)
    {
        if (arg.Exception != null)
        {
            this._logger.LogCritical(new(arg.Exception.HResult), exception: arg.Exception, message: arg.Message);
        }
        else
        {
            this._logger.LogCritical(arg.Message);
        }

        return Task.CompletedTask;
    }
}