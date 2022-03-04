using System.Diagnostics;

namespace Credfeto.Notification.Bot.Pgsql;

/// <summary>
///     Configuration for creating Postgresql connections.
/// </summary>
[DebuggerDisplay("{ConnectionString}")]
public sealed class PgsqlServerConfiguration
{
    /// <summary>
    ///     Gets the connection string.
    /// </summary>
    private string ConnectionString { get; init; } = default!;
}