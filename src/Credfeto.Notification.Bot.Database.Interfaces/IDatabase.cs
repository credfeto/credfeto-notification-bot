using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Database.Interfaces.Builders;

namespace Credfeto.Notification.Bot.Database.Interfaces;

/// <summary>
///     SQL Server Database.
/// </summary>
public interface IDatabase
{
    /// <summary>
    ///     Execute a stored procedure
    /// </summary>
    /// <param name="storedProcedure">The stored proc to execute</param>
    /// <returns>The number of rows affected</returns>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<int> ExecuteAsync(string storedProcedure);

    /// <summary>
    ///     Execute a stored procedure
    /// </summary>
    /// <typeparam name="TQueryParameters">The query parameters type.</typeparam>
    /// <param name="storedProcedure">The stored proc to execute</param>
    /// <param name="param">The parameters for the stored proc</param>
    /// <returns>The number of rows affected</returns>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<int> ExecuteAsync<TQueryParameters>(string storedProcedure, TQueryParameters param);

    /// <summary>
    ///     Executes an arbitrary piece of sql SQL against the database
    /// </summary>
    /// <param name="sql">The sql to execute</param>
    /// <returns>The number of rows affected</returns>
    /// <remarks>All updates should be executed using a stored procedure so use <see cref="ExecuteAsync" /> instead.</remarks>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<int> ExecuteArbitrarySqlAsync(string sql);

    /// <summary>
    ///     Execute a sql statement with parameters and return a single <typeparamref name="TResult" />,
    /// </summary>
    /// <typeparam name="TResult">The type to return</typeparam>
    /// <typeparam name="TSourceObject">The first type to map</typeparam>
    /// <param name="builder">The object builder that will convert from a <typeparamref name="TSourceObject" /> to a <typeparamref name="TResult" />.</param>
    /// <param name="storedProcedure">The sql to execute</param>
    /// <returns>The <typeparamref name="TResult" />.</returns>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<TResult> QuerySingleAsync<TSourceObject, TResult>(IObjectBuilder<TSourceObject, TResult> builder, string storedProcedure)
        where TSourceObject : class, new() where TResult : class;

    /// <summary>
    ///     Execute a sql statement with parameters and return a single <typeparamref name="TResult" />,
    /// </summary>
    /// <typeparam name="TQueryParameters">The query parameters type.</typeparam>
    /// <typeparam name="TResult">The type to return</typeparam>
    /// <typeparam name="TSourceObject">The first type to map</typeparam>
    /// <param name="builder">The object builder that will convert from a <typeparamref name="TSourceObject" /> to a <typeparamref name="TResult" />.</param>
    /// <param name="storedProcedure">The sql to execute</param>
    /// <param name="param">The parameters</param>
    /// <returns>The <typeparamref name="TResult" />.</returns>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<TResult> QuerySingleAsync<TQueryParameters, TSourceObject, TResult>(IObjectBuilder<TSourceObject, TResult> builder, string storedProcedure, TQueryParameters param)
        where TSourceObject : class, new() where TResult : class;

    /// <summary>
    ///     Execute a sql statement with parameters and return a single <typeparamref name="TResult" />,
    /// </summary>
    /// <typeparam name="TResult">The type to return</typeparam>
    /// <typeparam name="TSourceObject">The first type to map</typeparam>
    /// <param name="builder">The object builder that will convert from a <typeparamref name="TSourceObject" /> to a <typeparamref name="TResult" />.</param>
    /// <param name="storedProcedure">The sql to execute</param>
    /// <returns>The <typeparamref name="TResult" />.</returns>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<TResult?> QuerySingleOrDefaultAsync<TSourceObject, TResult>(IObjectBuilder<TSourceObject, TResult> builder, string storedProcedure)
        where TSourceObject : class, new() where TResult : class;

    /// <summary>
    ///     Execute a sql statement with parameters and return a single <typeparamref name="TResult" />,
    /// </summary>
    /// <typeparam name="TQueryParameters">The query parameters type.</typeparam>
    /// <typeparam name="TResult">The type to return</typeparam>
    /// <typeparam name="TSourceObject">The first type to map</typeparam>
    /// <param name="builder">The object builder that will convert from a <typeparamref name="TSourceObject" /> to a <typeparamref name="TResult" />.</param>
    /// <param name="storedProcedure">The sql to execute</param>
    /// <param name="param">The parameters</param>
    /// <returns>The <typeparamref name="TResult" />.</returns>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<TResult?> QuerySingleOrDefaultAsync<TQueryParameters, TSourceObject, TResult>(IObjectBuilder<TSourceObject, TResult> builder, string storedProcedure, TQueryParameters param)
        where TSourceObject : class, new() where TResult : class;

    /// <summary>
    ///     Execute a sql statement with parameters and return a single <typeparamref name="TResult" />,
    /// </summary>
    /// <typeparam name="TResult">The type to return</typeparam>
    /// <typeparam name="TSourceObject">The first type to map</typeparam>
    /// <param name="builder">The object builder that will convert from a <typeparamref name="TSourceObject" /> to a <typeparamref name="TResult" />.</param>
    /// <param name="storedProcedure">The sql to execute</param>
    /// <returns>Collection of <typeparamref name="TResult" />.</returns>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<IReadOnlyList<TResult>> QueryAsync<TSourceObject, TResult>(IObjectCollectionBuilder<TSourceObject, TResult> builder, string storedProcedure)
        where TSourceObject : class, new() where TResult : class;

    /// <summary>
    ///     Execute a sql statement with parameters and return a single <typeparamref name="TResult" />,
    /// </summary>
    /// <typeparam name="TQueryParameters">The query parameters type.</typeparam>
    /// <typeparam name="TResult">The type to return</typeparam>
    /// <typeparam name="TSourceObject">The first type to map</typeparam>
    /// <param name="builder">The object builder that will convert from a <typeparamref name="TSourceObject" /> to a <typeparamref name="TResult" />.</param>
    /// <param name="storedProcedure">The sql to execute</param>
    /// <param name="param">The parameters</param>
    /// <returns>Collection of <typeparamref name="TResult" />.</returns>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<IReadOnlyList<TResult>> QueryAsync<TQueryParameters, TSourceObject, TResult>(IObjectCollectionBuilder<TSourceObject, TResult> builder, string storedProcedure, TQueryParameters param)
        where TSourceObject : class, new() where TResult : class;

    /// <summary>
    ///     Query an arbitrary piece of sql SQL against the database
    /// </summary>
    /// <param name="sql">The sql to execute</param>
    /// <returns>Collection of <typeparamref name="TResult" />.</returns>
    /// <remarks>All queries should be executed using a stored procedure so use one of the QueryAsync methods instead.</remarks>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<IReadOnlyList<TResult>> QueryArbitrarySqlAsync<TResult>(string sql)
        where TResult : new();
}