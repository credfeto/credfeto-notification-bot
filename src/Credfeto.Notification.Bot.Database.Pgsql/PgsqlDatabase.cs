using System;
using System.Data;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Polly;

namespace Credfeto.Notification.Bot.Database.Pgsql;

public sealed class PgsqlDatabase : Dapper.Database
{
    private readonly PgsqlServerConfiguration _configuration;
    private readonly ILogger<PgsqlDatabase> _logger;

    public PgsqlDatabase(IOptions<PgsqlServerConfiguration> configuration, ILogger<PgsqlDatabase> logger)
    {
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this._configuration = configuration.Value ?? throw new ArgumentNullException(nameof(configuration));
    }

    protected override bool IsTransientException(Exception exception)
    {
        return exception is NpgsqlException { IsTransient: true };
    }

    protected override void LogAndDispatchTransientExceptions(Exception exception, Context context, in TimeSpan delay, int retryCount, int maxRetries)
    {
        string details = FormatException(exception: exception, context: context.OperationKey);

        this._logger.LogWarning(new(exception.HResult),
                                exception: exception,
                                $"Retrying transient exception {exception.GetType().Name}, on attempt {retryCount} of {maxRetries}. Current delay is {delay}: {details}");
    }

    private static string FormatException(Exception exception, string context)
    {
        int error = 0;

        StringBuilder sb = new StringBuilder().Append("Calling Stored Procedure: ")
                                              .AppendLine(context)
                                              .Append(++error);

        if (exception is NpgsqlException sqlException)
        {
            sb = sb.Append(": Error ")
                   .Append(sqlException.SqlState)
                   .Append(". Proc: ")
                   .Append(sqlException.BatchCommand)
                   .Append(": ")
                   .AppendLine(sqlException.Message);
        }

        return sb.ToString();
    }

    protected override IDbConnection GetConnection()
    {
        return new NpgsqlConnection(this._configuration.ConnectionString);
    }
}