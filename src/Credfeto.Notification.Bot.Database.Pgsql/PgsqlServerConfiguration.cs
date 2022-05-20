using System.Diagnostics;

namespace Credfeto.Notification.Bot.Database.Pgsql;

[DebuggerDisplay("{ConnectionString}")]
public sealed class PgsqlServerConfiguration
{
    public string ConnectionString { get; init; } = default!;
}