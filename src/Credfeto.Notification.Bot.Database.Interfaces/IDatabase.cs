using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Database.Interfaces.Builders;

namespace Credfeto.Notification.Bot.Database.Interfaces;

public interface IDatabase
{
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<int> ExecuteAsync(string storedProcedure);

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<int> ExecuteAsync<TQueryParameters>(string storedProcedure, TQueryParameters param);

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<int> ExecuteArbitrarySqlAsync(string sql);

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<TResult> QuerySingleAsync<TSourceObject, TResult>(IObjectBuilder<TSourceObject, TResult> builder, string storedProcedure)
        where TSourceObject : class, new() where TResult : class;

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<TResult> QuerySingleAsync<TQueryParameters, TSourceObject, TResult>(IObjectBuilder<TSourceObject, TResult> builder, string storedProcedure, TQueryParameters param)
        where TSourceObject : class, new() where TResult : class;

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<TResult?> QuerySingleOrDefaultAsync<TSourceObject, TResult>(IObjectBuilder<TSourceObject, TResult> builder, string storedProcedure)
        where TSourceObject : class, new() where TResult : class;

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<TResult?> QuerySingleOrDefaultAsync<TQueryParameters, TSourceObject, TResult>(IObjectBuilder<TSourceObject, TResult> builder, string storedProcedure, TQueryParameters param)
        where TSourceObject : class, new() where TResult : class;

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<IReadOnlyList<TResult>> QueryAsync<TSourceObject, TResult>(IObjectCollectionBuilder<TSourceObject, TResult> builder, string storedProcedure)
        where TSourceObject : class, new() where TResult : class;

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<IReadOnlyList<TResult>> QueryAsync<TQueryParameters, TSourceObject, TResult>(IObjectCollectionBuilder<TSourceObject, TResult> builder, string storedProcedure, TQueryParameters param)
        where TSourceObject : class, new() where TResult : class;

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    Task<IReadOnlyList<TResult>> QueryArbitrarySqlAsync<TResult>(string sql)
        where TResult : new();
}