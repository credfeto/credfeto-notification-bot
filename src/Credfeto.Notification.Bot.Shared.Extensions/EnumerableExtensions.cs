using System.Collections.Generic;

namespace Credfeto.Notification.Bot.Shared.Extensions;

public static class EnumerableExtensions
{
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