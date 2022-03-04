using Credfeto.Notification.Bot.Database.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Notification.Bot.Database.Pgsql;

/// <summary>
///     Configures Postgresql DB
/// </summary>
public static class PostgresqlSetup
{
    /// <summary>
    ///     Configures Postgresql DB.
    /// </summary>
    /// <param name="services"></param>
    public static IServiceCollection ConfigurePostgresql(this IServiceCollection services)
    {
        return services.AddSingleton<IDatabase, PgsqlDatabase>();
    }
}