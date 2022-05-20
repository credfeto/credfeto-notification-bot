using Credfeto.Notification.Bot.Database.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Credfeto.Notification.Bot.Database.Pgsql;

public static class PostgresqlSetup
{
    public static IServiceCollection AddPostgresql(this IServiceCollection services)
    {
        return services.AddSingleton<IDatabase, PgsqlDatabase>();
    }
}