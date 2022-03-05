using System.Collections.Generic;

namespace Credfeto.Notification.Bot.Shared.Extensions;

/// <summary>
///     Extensions on <see cref="IEnumerable{T}" />
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    ///     Removes nulls from the source collection.
    /// </summary>
    /// <param name="source">The source collection</param>
    /// <typeparam name="TItemType">The item type</typeparam>
    /// <returns>Collection without nulls.</returns>
    public static IEnumerable<TItemType> RemoveNulls<TItemType>(this IEnumerable<TItemType?> source)
        where TItemType : class
    {
        foreach (TItemType? item in source)
        {
            if (!ReferenceEquals(objA: item, objB: null))
            {
                yield return item;
            }
        }
    }
}