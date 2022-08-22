using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Discord;
using Microsoft.Extensions.Logging;

namespace Credfeto.Notification.Bot.Discord.Services;

public abstract class DiscordLoggingBase
{
    private readonly ILogger _logger;

    protected DiscordLoggingBase(ILogger logger)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

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

    [SuppressMessage(category: "codecracker.CSharp", checkId: "CC0091: Make method static", Justification = "Conditional code in debug builds")]
    private Task LogDebugAsync(LogMessage arg)
    {
        this.IssueDebugLog(arg);

        return Task.CompletedTask;
    }

    [Conditional("DEBUG")]
    private void IssueDebugLog(LogMessage arg)
    {
        if (arg.Exception != null)
        {
            this._logger.LogDebug(new(arg.Exception.HResult), exception: arg.Exception, message: arg.Message);
        }
        else
        {
            this._logger.LogDebug(arg.Message);
        }
    }

    [SuppressMessage(category: "codecracker.CSharp", checkId: "CC0091: Make method static", Justification = "Conditional code in debug builds")]
    private Task LogInformationAsync(LogMessage arg)
    {
        this.IssueInformationalLog(arg);

        return Task.CompletedTask;
    }

    [Conditional("DEBUG")]
    private void IssueInformationalLog(LogMessage arg)
    {
        if (arg.Exception != null)
        {
            this._logger.LogInformation(new(arg.Exception.HResult), exception: arg.Exception, message: arg.Message);
        }
        else
        {
            this._logger.LogInformation(arg.Message);
        }
    }

    [SuppressMessage(category: "codecracker.CSharp", checkId: "CC0091: Make method static", Justification = "Conditional code in debug builds")]
    private Task LogWarningAsync(LogMessage arg)
    {
        this.IssueWarningLog(arg);

        return Task.CompletedTask;
    }

    [Conditional("DEBUG")]
    private void IssueWarningLog(LogMessage arg)
    {
        if (arg.Exception != null)
        {
            this._logger.LogWarning(new(arg.Exception.HResult), exception: arg.Exception, message: arg.Message);
        }
        else
        {
            this._logger.LogWarning(arg.Message);
        }
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